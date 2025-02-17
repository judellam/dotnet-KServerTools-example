//
//  HomeView.swift
//  MyApp
//
//  Created by Justin Dellamore on 2/11/25.
//

import SwiftUI

struct HomeView: View {
    @State private var selectedSegment:Int = 1
    var body: some View {
        VStack (spacing: 10) {
            Picker("Options", selection: $selectedSegment) {
                Text("Register").tag(0)
                Text("Login").tag(1)
                Text("Test API").tag(2)
                Text("All Users").tag(3)
            }
            .pickerStyle(.segmented)
            .padding()
            .cornerRadius(12)
            .padding(.horizontal)
            .frame(maxWidth: .infinity)
            
            Spacer()
            
            if selectedSegment == 0 {
                RegisterView()
            }
            else if selectedSegment == 1 {
                LoginView()
            } else if selectedSegment == 2 {
                AuthorizedSampleView()
            } else if selectedSegment == 3 {
                AllUsersView()
            }
            
            Spacer()
        }
    }
}

#Preview {
    HomeView()
        .environmentObject(AuthManager.shared)
}
