//
//  MyAppApp.swift
//  MyApp
//
//  Created by Justin D on 2/11/25.
//

import SwiftUI

@main
struct MyAppApp: App {
    @StateObject private var authManager: AuthManager = AuthManager.shared
    var body: some Scene {
        WindowGroup {
            ContentView()
                .environmentObject(authManager)
        }
    }
}
