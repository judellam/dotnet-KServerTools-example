//
//  AuthorizedSampleView.swift
//  MyApp
//
//  Created by Justin Dellamore on 2/11/25.
//

import SwiftUI

struct AuthorizedSampleView: View {
    @EnvironmentObject var authManager: AuthManager
    @State private var responseText: String = ""
    @State private var isAnimating = false
    
    var body: some View {
        VStack (spacing: 10) {
            Image(systemName: "globe")
                .imageScale(.large)
                .foregroundStyle(isAnimating ? .white : .blue)
                .animation(.easeInOut(duration: 1), value: isAnimating)
            
            TextField("Response", text: $responseText)
                .textFieldStyle(.roundedBorder)
                .disabled(true)
                .padding()
            
            Button("Call GET API") {
                isAnimating = true
                Task {
                    try await fetchData()
                    isAnimating = false
                }
            }
            .padding()
            .frame(maxWidth: .infinity)
            .background(Color.blue)
            .foregroundColor(.white)
            .cornerRadius(10)
            .padding(.horizontal)
        }
        .padding()
    }
    
    private func fetchData() async throws {
        let token = authManager.token ?? ""
        var headers: [String:String] = [:]
        if !token.isEmpty {
            headers = NetworkHelper.getAuthorizationHeader(token: token)
        }
        
        do {
            let endpoint = "\(Constants.LocalEndpoint)/sample"
            let response: SampleResponse = try await NetworkHelper.getRequest(url: endpoint, headers)
            responseText = response.message
        } catch {
            print ("Error: \(error)")
        }
    }
}

#Preview {
    AuthorizedSampleView()
        .environmentObject(AuthManager.shared)
}
