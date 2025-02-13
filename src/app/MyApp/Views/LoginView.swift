//
//  LoginView.swift
//  MyApp
//
//  Created by Justin D on 2/10/25.
//

import SwiftUI

struct LoginView: View {
    @EnvironmentObject var authManager: AuthManager
    
    @State private var username: String = UserDefaults.standard.string(forKey: Constants.LoginUserName) ?? ""
    @State private var password: String = UserDefaults.standard.string(forKey: Constants.LoginUserPassword) ?? ""
    @State private var loginStatus: String = ""
    
    var body: some View {
        VStack(spacing: 20) {
            TextField("Username", text: $username)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .autocapitalization(.none)
                .padding(.horizontal)

            SecureField("Password", text: $password)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .padding(.horizontal)

            Button("Login") {
                Task {
                    await login()
                }
            }
            .padding()
            .frame(maxWidth: .infinity)
            .background(Color.blue)
            .foregroundColor(.white)
            .cornerRadius(10)
            .padding(.horizontal)

            Text(loginStatus)
                .foregroundColor(authManager.isLoggedIn ? .green : .red)
        }
        .padding()
        .onAppear {
            loginStatus = authManager.isLoggedIn ? Constants.LoginSuccess : ""
        }
    }
    
    func login() async {
        do {
            let credentials = CredentialsRequest(username: username, password: password)
            let endpoint: String = "\(Constants.LocalEndpoint)/user/login"
            let response:LoginResponse = try await NetworkHelper.postRequest(url: endpoint, credentials)
            
            if response.jwtToken != "" {
                UserDefaults.standard.set(username, forKey: Constants.LoginUserName)
                UserDefaults.standard.set(password, forKey: Constants.LoginUserPassword)
            }
            
            DispatchQueue.main.async {
                let success = response.jwtToken != ""
                loginStatus = success ? Constants.LoginSuccess : Constants.LoginFailure
                authManager.isLoggedIn = success
                authManager.token = success ? response.jwtToken : ""
            }
        } catch {
            print(error.localizedDescription)
            DispatchQueue.main.async {
                loginStatus = Constants.LoginFailure
                authManager.isLoggedIn = false
            }
        }
    }
}

#Preview {
    LoginView()
        .environmentObject(AuthManager.shared)
}
