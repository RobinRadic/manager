using System.Text;

namespace Manager.Core.Features.Nginx.Configuration;

public class NginxLexer
{
    private readonly string _source;
    private int _pos;
    private int _line = 1;
    private int _col = 1;

    public NginxLexer(string source)
    {
        _source = source;
        _pos = 0;
    }

    internal Token NextToken()
    {
        SkipWhitespace();

        if (_pos >= _source.Length)
            return new Token { Type = TokenType.EOF, Line = _line, Column = _col };

        char current = _source[_pos];

        // 1. Handle Comments
        if (current == '#')
        {
            return ReadComment();
        }

        // 2. Handle Syntax Chars
        if (current == '{') return AdvanceChar(TokenType.OpenBrace, "{");
        if (current == '}') return AdvanceChar(TokenType.CloseBrace, "}");
        if (current == ';') return AdvanceChar(TokenType.Semicolon, ";");

        // 3. Handle Quoted Strings
        if (current == '"' || current == '\'')
        {
            return ReadQuotedString(current);
        }

        // 4. Handle Identifiers / Bare words
        return ReadIdentifier();
    }

    private void SkipWhitespace()
    {
        while (_pos < _source.Length)
        {
            char c = _source[_pos];
            if (char.IsWhiteSpace(c))
            {
                if (c == '\n')
                {
                    _line++;
                    _col = 1;
                }
                else
                {
                    _col++;
                }

                _pos++;
            }
            else
            {
                break;
            }
        }
    }

    private Token ReadComment()
    {
        int startCol = _col;
        _pos++; // Skip #
        _col++;

        var sb = new StringBuilder();
        while (_pos < _source.Length && _source[_pos] != '\n' && _source[_pos] != '\r')
        {
            sb.Append(_source[_pos]);
            _pos++;
            _col++;
        }

        return new Token { Type = TokenType.Comment, Value = sb.ToString(), Line = _line, Column = startCol };
    }

    private Token AdvanceChar(TokenType type, string val)
    {
        var t = new Token { Type = type, Value = val, Line = _line, Column = _col };
        _pos++;
        _col++;
        return t;
    }

    private Token ReadQuotedString(char quoteType)
    {
        int startCol = _col;
        _pos++; // Skip opening quote
        _col++;

        var sb = new StringBuilder();
        bool escaped = false;

        while (_pos < _source.Length)
        {
            char c = _source[_pos];

            if (escaped)
            {
                sb.Append(c);
                escaped = false;
            }
            else if (c == '\\')
            {
                escaped = true; // Next char is literal
            }
            else if (c == quoteType)
            {
                _pos++; // Skip closing quote
                _col++;
                return new Token { Type = TokenType.Identifier, Value = sb.ToString(), Line = _line, Column = startCol }; // Treat string as Identifier for parser simplicity
            }
            else
            {
                sb.Append(c);
            }

            _pos++;
            _col++;

            if (c == '\n')
            {
                _line++;
                _col = 1;
            }
        }

        throw new Exception($"Unterminated string starting at line {_line}");
    }

    private Token ReadIdentifier()
    {
        int startCol = _col;
        var sb = new StringBuilder();

        while (_pos < _source.Length)
        {
            char c = _source[_pos];

            // Break on delimiters
            if (char.IsWhiteSpace(c) || c == '{' || c == '}' || c == ';' || c == '#')
            {
                break;
            }

            sb.Append(c);
            _pos++;
            _col++;
        }

        return new Token { Type = TokenType.Identifier, Value = sb.ToString(), Line = _line, Column = startCol };
    }
}