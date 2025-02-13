//
//  LoginResponse.swift
//  MyApp
//
//  Created by Justin D on 2/11/25.
//

struct LoginResponse: Decodable {
    let jwtToken: String
    private enum CodingKeys: String, CodingKey {
        case jwtToken = "jwt-token"
    }
}
