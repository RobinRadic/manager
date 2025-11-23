namespace Manager.Core.Features.Nginx.Configuration;

public class NginxParser
{
    private NginxLexer _lexer;
    private Token _currentToken;

    public static NginxConfig fromSource(string source)
    {
        var parser = new NginxParser();
        return parser.Parse(source);
    }

    public NginxConfig Parse(string source)
    {
        _lexer = new NginxLexer(source);
        Advance();

        var config = new NginxConfig();

        while (_currentToken.Type != TokenType.EOF)
        {
            config.RootNodes.Add(ParseStatement());
        }

        return config;
    }

    private void Advance()
    {
        _currentToken = _lexer.NextToken();
    }

    private NginxNode ParseStatement()
    {
        if (_currentToken.Type == TokenType.Comment)
        {
            var commentNode = new NginxComment(_currentToken.Value);
            Advance();
            return commentNode;
        }

        if (_currentToken.Type != TokenType.Identifier)
        {
            throw new Exception($"Unexpected token {_currentToken.Type} at Line {_currentToken.Line}, Col {_currentToken.Column}. Expected Directive Name.");
        }

        string directiveName = _currentToken.Value;
        var directive = new NginxDirective(directiveName);
        Advance();

        // Read Arguments
        while (_currentToken.Type == TokenType.Identifier)
        {
            directive.AddArgument(_currentToken.Value);
            Advance();
        }

        // Check for Block Start or Line End
        if (_currentToken.Type == TokenType.Semicolon)
        {
            // Simple directive: "worker_processes 1;"
            Advance();
            return directive;
        }
        else if (_currentToken.Type == TokenType.OpenBrace)
        {
            // Block directive: "http { ... }"
            Advance(); // Skip {
            directive.Children = new List<NginxNode>();

            while (_currentToken.Type != TokenType.CloseBrace && _currentToken.Type != TokenType.EOF)
            {
                directive.Children.Add(ParseStatement());
            }

            if (_currentToken.Type == TokenType.EOF)
                throw new Exception($"Unexpected EOF inside block '{directiveName}'. Missing closing brace }}?");

            Advance(); // Skip }
            return directive;
        }
        else
        {
            throw new Exception($"Unexpected token {_currentToken.Value} at Line {_currentToken.Line}. Expected ';' or '{{'.");
        }
    }
}