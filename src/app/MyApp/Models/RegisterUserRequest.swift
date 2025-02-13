//
//  RegisterUserRequest.swift
//  MyApp
//
//  Created by Justin Dellamore on 2/11/25.
//

struct RegisterUserRequest: Encodable, Decodable {
    let userName: String;
    let password: String
    let email: String
}
