namespace server;
public static class Constants {
    public static class Jwt {
        public const string Secret = "example-secret-key-of-muscle-power-and-awesomeness"; // pick up from a secret store like AKV
        public const string Issuer = "example-issuer";
        public const string Audience = "example-audience";
    }
}