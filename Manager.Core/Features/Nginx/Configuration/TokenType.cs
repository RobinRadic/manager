namespace Manager.Core.Features.Nginx.Configuration;

internal enum TokenType
{
    Identifier, // text, numbers, or unquoted strings
    String,     // Quoted text
    OpenBrace,  // {
    CloseBrace, // }
    Semicolon,  // ;
    Comment,    // # ...
    EOF
}