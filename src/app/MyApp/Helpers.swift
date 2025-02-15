//
//  Helpers.swift
//  MyApp
//
//  Created by Justin D on 2/11/25.
//

import Foundation

enum NetworkError: Error {
    case unauthorized  // 401
    case forbidden     // 403
    case unknown
}

class NetworkHelper {
    static func getRequest<T:Decodable>(url: String, _ headers: [String:String]? = nil) async throws -> T {
        let request = try createRequest(url: url, "GET", headers)
        let (data, response) = try await URLSession.shared.data(for: request)
        
        guard let httpResponse = response as? HTTPURLResponse else {
            throw URLError(.badServerResponse)
        }
        
        try handleResponse(httpResponse)
                
        return try JSONDecoder().decode(T.self, from: data)
    }
    
    static func postRequest<T: Decodable>(url: String, _ body: Encodable, _ headers: [String:String]? = nil) async throws -> T {
        var request = try createRequest(url: url, "POST", headers)
        
        request.httpBody = try JSONEncoder().encode(body)
        let (data, response) = try await URLSession.shared.data(for: request)
        
        guard let httpResponse = response as? HTTPURLResponse else {
            throw URLError(.badServerResponse)
        }
        
        try handleResponse(httpResponse)
        
        return try JSONDecoder().decode(T.self, from: data)
    }
    
    static func getAuthorizationHeader(token: String) -> [String:String] {
        return [Constants.AuthorizationHeader: "Bearer \(token)"]
    }
    
    private static func handleResponse(_ response: URLResponse) throws {
        guard let httpResponse = response as? HTTPURLResponse else {
            throw URLError(.badServerResponse)
        }
                
        if httpResponse.statusCode == 401 {
            throw NetworkError.unauthorized
        } else if httpResponse.statusCode == 403 {
            throw NetworkError.forbidden
        }
        
        guard (200...202).contains(httpResponse.statusCode) else {
            throw NetworkError.unknown
        }
    }
        
    private static func createRequest(url: String, _ method: String, _ headers: [String:String]? = nil) throws -> URLRequest {
        guard let url = URL(string: url) else {
            throw URLError(.badURL)
        }
        var request = URLRequest(url: url)
        request.addValue("application/json", forHTTPHeaderField: "Content-Type")
        request.httpMethod = method
        
        let headers = headers ?? [:]
        for (key, value) in headers {
            request.addValue(value, forHTTPHeaderField: key)
        }
        
        return request
    }
}
