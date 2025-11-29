using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace Manager.GUI.Services.Php;

public static class PhpDocumentation
{
    private static readonly IBrush ColorKeyword = SolidColorBrush.Parse("#569cd6");

    public static object GetTooltip(string keyword)
    {
        if (!_docs.TryGetValue(keyword, out var description)) return null;

        var panel = new StackPanel { Spacing = 5 };
        var border = new Border
        {
            Child = new TextBlock
            {
                Inlines = { new Run { Text = keyword, Foreground = ColorKeyword, FontWeight = FontWeight.Bold } },
                FontFamily = new FontFamily("Cascadia Code, Consolas, Monospace")
            },
            BorderBrush = SolidColorBrush.Parse("#3E3E42"),
            BorderThickness = new Avalonia.Thickness(0, 0, 0, 1),
            Padding = new Avalonia.Thickness(0, 0, 0, 8)
        };

        panel.Children.Add(border);
        panel.Children.Add(new TextBlock { Text = description, TextWrapping = TextWrapping.Wrap, FontSize = 12, Opacity = 0.8 });

        return panel;
    }

    private static readonly Dictionary<string, string> _docs = new Dictionary<string, string>
    {
        // Core
        { "display_errors", "Determines whether errors should be printed to the screen as part of the output." },
        { "error_reporting", "Set the error reporting level. Common values: E_ALL & ~E_NOTICE." },
        { "memory_limit", "Maximum amount of memory a script may consume." },
        { "max_execution_time", "Maximum time in seconds a script is allowed to run before being terminated." },
        { "upload_max_filesize", "The maximum size of an uploaded file." },
        { "post_max_size", "Sets max size of post data allowed. This setting also affects file upload. To upload large files, this value must be larger than upload_max_filesize." },
        { "date.timezone", "The default timezone used by all date/time functions." },
        { "short_open_tag", "Tells PHP whether the short form (<? ?>) of PHP's open tag should be allowed." },
        { "extension", "Load a dynamic extension." },
        { "zend_extension", "Load a Zend extension (e.g. Xdebug, Opcache)." },
        { "cgi.fix_pathinfo", "Provides real PATH_INFO/PATH_TRANSLATED support for CGI." },
        { "include_path", "Specifies a list of directories where the require, include, fopen(), file(), readfile() and file_get_contents() functions look for files." },
        { "expose_php", "Decides whether PHP may expose the fact that it is installed on the server (e.g., by adding its signature to the Web server header)." },
        { "file_uploads", "Whether to allow HTTP file uploads." },
        { "max_file_uploads", "The maximum number of files allowed to be uploaded simultaneously." },
        { "default_socket_timeout", "Default timeout (in seconds) for socket based streams." },
        
        // Session
        { "session.save_handler", "Handler used to store/retrieve data. Default is 'files'." },
        { "session.save_path", "Argument passed to the save_handler. For files, this is the path where data files are stored." },
        { "session.use_cookies", "Specifies whether the module will use cookies to store the session id on the client side." },
        { "session.cookie_lifetime", "Lifetime of the cookie in seconds which is sent to the browser." },
        { "session.gc_maxlifetime", "Specifies the number of seconds after which data will be seen as 'garbage' and potentially cleaned up." },

        // Opcache
        { "opcache.enable", "Enables the opcode cache." },
        { "opcache.memory_consumption", "The size of the shared memory storage used by OPcache." },
        { "opcache.interned_strings_buffer", "The amount of memory for interned strings in Mbytes." },
        { "opcache.max_accelerated_files", "The maximum number of keys (scripts) in the OPcache hash table." },
        { "opcache.revalidate_freq", "How often (in seconds) to check file timestamps for changes to the shared memory storage allocation." },
        { "opcache.validate_timestamps", "If enabled, OPcache will check for updated scripts every opcache.revalidate_freq seconds." },
        
        // Xdebug
        { "xdebug.mode", "Controls which Xdebug features are enabled (develop, debug, profile, trace, etc)." },
        { "xdebug.client_host", "The IP or hostname where the Xdebug client (IDE) is listening." }
    };
}