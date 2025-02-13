//
//  AuthManager.swift
//  MyApp
//
//  Created by Justin D on 2/11/25.
//
import Foundation

class AuthManager: ObservableObject {
    @Published var isLoggedIn: Bool = false
    @Published var token: String? = nil
    static let shared = AuthManager()
    private init() {}
}
