//
//  AllUsersView.swift
//  MyApp
//
//  Created by Justin D on 2/16/25.
//

import SwiftUI

struct AllUsersView: View {
    @State var users: [User] = []
    @EnvironmentObject var authManager: AuthManager
    
    var body: some View {
        ScrollView {
            LazyVStack {
                ForEach(users, id: \.userName) { user in
                    VStack(alignment: .leading) {
                        Text(user.userName)
                            .font(.headline)
                        Text(user.email)
                            .font(.subheadline)
                            .foregroundColor(.gray)
                    }
                    .padding()
                    .frame(maxWidth: .infinity, alignment: .leading)
                    .background(Color.gray.opacity(0.2))
                    .cornerRadius(8)
                }
            }
            .padding()
        }
        
        Button("Get All Users") {
            Task {
                try await fetchUsers()
            }
        }
        .padding()
        .frame(maxWidth: .infinity)
        .background(Color.blue)
        .foregroundColor(.white)
        .cornerRadius(10)
        .padding(.horizontal)
        
        Spacer()
    }
    
    private func fetchUsers() async throws {
        let token = authManager.token ?? ""
        var headers: [String:String] = [:]
        if !token.isEmpty {
            headers = NetworkHelper.getAuthorizationHeader(token: token)
        }
        
        do {
            let endpoint = "\(Constants.LocalEndpoint)/user/all"
            users = try await NetworkHelper.getRequest(url: endpoint, headers)
        } catch {
            print ("Error: \(error)")
        }
    }
}

#Preview {
    AllUsersView(users: [User(userName: "a", password: "", email: "a@a.com"), User(userName: "b", password: "", email: "b@a.com")])
        .environmentObject(AuthManager.shared)
}
