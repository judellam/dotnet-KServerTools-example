//
//  RegisterView.swift
//  MyApp
//
//  Created by Justin D on 2/11/25.
//

import SwiftUI

struct RegisterView: View {
    @State private var userName: String = ""
    @State private var password: String = ""
    @State private var email: String = ""
    @State private var registerStatus: String = ""
    @State private var success: Bool = false
    
    var body: some View {
        VStack(spacing: 20) {
            TextField("Username", text: $userName)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .autocapitalization(.none)
                .padding(.horizontal)

            SecureField("Password", text: $password)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .padding(.horizontal)
            
            TextField("Email", text: $email)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .autocapitalization(.none)
                .padding(.horizontal)

            Button("Register") {
                Task {
                    try await registerUser()
                }
            }
            .padding()
            .frame(maxWidth: .infinity)
            .background(Color.blue)
            .foregroundColor(.white)
            .cornerRadius(10)
            .padding(.horizontal)

            Text(registerStatus)
                .foregroundColor(success ? .green : .red)
        }
        .padding()
    }
    
    private func registerUser() async throws {
        let endpoint = "\(Constants.LocalEndpoint)/user/register"
        let registerUser = RegisterUserRequest(userName: userName, password: password, email: email)
        do {
            let response: RegisterUserRequest = try await NetworkHelper.postRequest(url: endpoint, registerUser)
            DispatchQueue.main.async {
                self.success = true
                self.registerStatus = "Registered \(response.userName)"
            }
        } catch {
            DispatchQueue.main.async {
                self.success = false
                self.registerStatus = "Registration failed: \(error.localizedDescription)"
            }
        }
    }
}

#Preview {
    RegisterView()
}
