using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace Manager.GUI.Services.Nginx;

public static class NginxDocumentation
{
// Colors for the Syntax Preview (VS Code Dark Theme style)
    private static readonly IBrush ColorDirective = SolidColorBrush.Parse("#569cd6"); // Blue
    private static readonly IBrush ColorArgs = SolidColorBrush.Parse("#ce9178"); // Orange/String color
    private static readonly IBrush ColorVariable = SolidColorBrush.Parse("#c586c0"); // Purple
    private static readonly IBrush ColorPunctuation = SolidColorBrush.Parse("#d4d4d4"); // Whiteish

    /// <summary>
    /// Generates a rich UI tooltip with color-coded syntax.
    /// </summary>
    public static object GetTooltip(string keyword)
    {
        if (!_docs.TryGetValue(keyword, out var item)) return null;

        var panel = new StackPanel { Spacing = 5 };

        // 1. Syntax Header (Color Coded)
        var syntaxBlock = new TextBlock
        {
            FontFamily = new FontFamily("Cascadia Code, Consolas, Monospace"),
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap
        };

        var inlines = new InlineCollection();

        // Simple parser for coloring
        if (keyword.StartsWith("$"))
        {
            // It's a variable
            inlines.Add(new Run { Text = item.Syntax, Foreground = ColorVariable, FontWeight = FontWeight.Bold });
        }
        else
        {
            // It's a directive
            string[] parts = item.Syntax.Split(' ', 2);

            // Directive Name (Blue)
            inlines.Add(new Run { Text = parts[0], Foreground = ColorDirective, FontWeight = FontWeight.Bold });

            // Arguments (Orange) - if any
            if (parts.Length > 1)
            {
                inlines.Add(new Run { Text = " " + parts[1], Foreground = ColorArgs });
            }
        }

        syntaxBlock.Inlines = inlines;

        // Add a bottom border to the syntax header
        var border = new Border
        {
            Child = syntaxBlock,
            BorderBrush = SolidColorBrush.Parse("#3E3E42"),
            BorderThickness = new Avalonia.Thickness(0, 0, 0, 1),
            Padding = new Avalonia.Thickness(0, 0, 0, 8),
            Margin = new Avalonia.Thickness(0, 0, 0, 5)
        };

        // 2. Description Body
        var descBlock = new TextBlock
        {
            Text = item.Description,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Opacity = 0.8
        };

        panel.Children.Add(border);
        panel.Children.Add(descBlock);

        return panel;
    }

    private static readonly Dictionary<string, NginxDocItem> _docs = new Dictionary<string, NginxDocItem>
    {
        // --- Core & Global ---
        { "daemon", new NginxDocItem { Syntax = "daemon on | off;", Description = "Determines whether nginx should become a daemon. Mainly used during development." } },
        { "env", new NginxDocItem { Syntax = "env variable[=value];", Description = "By default, nginx removes all environment variables inherited from its parent process except the TZ variable. This directive allows preserving some of the inherited variables, changing their values, or creating new environment variables." } },
        { "debug_points", new NginxDocItem { Syntax = "debug_points abort | stop;", Description = "This directive is used for debugging. When internal error is detected, e.g. leak of sockets, nginx will either stop (abort) and create a core file, or stop (stop) the process." } },
        { "error_log", new NginxDocItem { Syntax = "error_log file [level];", Description = "Configures logging. The first parameter defines a file that will store the log. The second parameter determines the level of logging (debug, info, notice, warn, error, crit, alert, or emerg)." } },
        { "include", new NginxDocItem { Syntax = "include file | mask;", Description = "Includes another file, or files matching the specified mask, into configuration. Included files should consist of syntactically correct directives and blocks." } },
        { "lock_file", new NginxDocItem { Syntax = "lock_file file;", Description = "nginx uses the accept mutex to serialize accept() syscalls. This directive sets the prefix for the lock file name." } },
        { "master_process", new NginxDocItem { Syntax = "master_process on | off;", Description = "Determines whether worker processes are started. This directive is intended for nginx developers." } },
        { "pid", new NginxDocItem { Syntax = "pid file;", Description = "Defines a file that will store the process ID of the main process." } },
        { "ssl_engine", new NginxDocItem { Syntax = "ssl_engine device;", Description = "Defines the name of the hardware SSL accelerator." } },
        { "timer_resolution", new NginxDocItem { Syntax = "timer_resolution interval;", Description = "Reduces timer resolution in worker processes, thus reducing the number of gettimeofday() system calls made." } },
        { "user", new NginxDocItem { Syntax = "user user [group];", Description = "Defines user and group credentials used by worker processes. If group is omitted, a group whose name equals that of user is used." } },
        { "worker_cpu_affinity", new NginxDocItem { Syntax = "worker_cpu_affinity cpumask ...;", Description = "Binds worker processes to the sets of CPUs. Each CPU set is represented by a bitmask of allowed CPUs." } },
        { "worker_priority", new NginxDocItem { Syntax = "worker_priority number;", Description = "Defines the scheduling priority for worker processes like is done by the nice(1) command: a negative number means higher priority." } },
        { "worker_processes", new NginxDocItem { Syntax = "worker_processes number | auto;", Description = "Defines the number of worker processes. The optimal value depends on many factors including (but not limited to) the number of CPU cores, the number of hard disk drives that store data, and load pattern." } },
        { "worker_rlimit_core", new NginxDocItem { Syntax = "worker_rlimit_core size;", Description = "Changes the limit on the largest size of a core file (RLIMIT_CORE) for worker processes." } },
        { "worker_rlimit_nofile", new NginxDocItem { Syntax = "worker_rlimit_nofile number;", Description = "Changes the limit on the maximum number of open files (RLIMIT_NOFILE) for worker processes." } },
        { "worker_rlimit_sigpending", new NginxDocItem { Syntax = "worker_rlimit_sigpending number;", Description = "Changes the limit on the number of signals that may be queued for the real user ID of the calling process (RLIMIT_SIGPENDING) for worker processes." } },
        { "working_directory", new NginxDocItem { Syntax = "working_directory directory;", Description = "Defines the current working directory for a worker process. It is primarily used when writing a core-file." } },
        { "load_module", new NginxDocItem { Syntax = "load_module file;", Description = "Loads a dynamic module." } },

        // --- Events Module ---
        { "events", new NginxDocItem { Syntax = "events { ... }", Description = "Provides the configuration file context in which the directives that affect connection processing are specified." } },
        { "accept_mutex", new NginxDocItem { Syntax = "accept_mutex on | off;", Description = "If accept_mutex is enabled, worker processes will accept new connections by turn. Otherwise, all worker processes will be notified about new connections, and if volume of new connections is low, some of the worker processes may just waste system resources." } },
        { "accept_mutex_delay", new NginxDocItem { Syntax = "accept_mutex_delay time;", Description = "If accept_mutex is enabled, specifies the maximum time during which a worker process will try to restart accepting new connections if another worker process is currently accepting new connections." } },
        { "debug_connection", new NginxDocItem { Syntax = "debug_connection address | CIDR | unix:;", Description = "Enables debugging log for selected client connections." } },
        { "devpoll_changes", new NginxDocItem { Syntax = "devpoll_changes number;", Description = "Sets the number of events passed to the kernel in one system call." } },
        { "devpoll_events", new NginxDocItem { Syntax = "devpoll_events number;", Description = "Sets the number of events passed from the kernel in one system call." } },
        { "epoll_events", new NginxDocItem { Syntax = "epoll_events number;", Description = "Sets the number of events passed from the kernel in one system call." } },
        { "kqueue_changes", new NginxDocItem { Syntax = "kqueue_changes number;", Description = "Sets the number of events passed to the kernel in one system call." } },
        { "kqueue_events", new NginxDocItem { Syntax = "kqueue_events number;", Description = "Sets the number of events passed from the kernel in one system call." } },
        { "multi_accept", new NginxDocItem { Syntax = "multi_accept on | off;", Description = "If multi_accept is disabled, a worker process will accept one new connection at a time. Otherwise, a worker process will accept all new connections at a time." } },
        { "rtsig_signo", new NginxDocItem { Syntax = "rtsig_signo signal;", Description = "The directive specifies the first signal to use in the rtsig method." } },
        { "rtsig_overflow_events", new NginxDocItem { Syntax = "rtsig_overflow_events number;", Description = "Specifies the number of events to be passed via poll() when the queue of rtsig overflows." } },
        { "rtsig_overflow_test", new NginxDocItem { Syntax = "rtsig_overflow_test number;", Description = "Specifies after how many events handled via rtsig nginx enters the queue overflow test mode." } },
        { "rtsig_overflow_threshold", new NginxDocItem { Syntax = "rtsig_overflow_threshold number;", Description = "Specifies the queue logic for rtsig." } },
        { "use", new NginxDocItem { Syntax = "use method;", Description = "Specifies the connection processing method to use (e.g., epoll, kqueue, select, poll)." } },
        { "worker_connections", new NginxDocItem { Syntax = "worker_connections number;", Description = "Sets the maximum number of simultaneous connections that can be opened by a worker process." } },

        // --- HTTP Core ---
        { "http", new NginxDocItem { Syntax = "http { ... }", Description = "Provides the configuration file context in which the HTTP server directives are specified." } },
        { "server", new NginxDocItem { Syntax = "server { ... }", Description = "Sets configuration for a virtual server." } },
        { "location", new NginxDocItem { Syntax = "location [ = | ~ | ~* | ^~ ] uri { ... }", Description = "Sets configuration depending on a request URI." } },
        { "root", new NginxDocItem { Syntax = "root path;", Description = "Sets the root directory for requests. For example, with 'root /i/;', the request '/i/top.gif' will try '/i/top.gif'." } },
        { "alias", new NginxDocItem { Syntax = "alias path;", Description = "Defines a replacement for the specified location. For example, with 'location /i/ { alias /data/w3/images/; }', the request '/i/top.gif' will try '/data/w3/images/top.gif'." } },
        { "index", new NginxDocItem { Syntax = "index file ...;", Description = "Defines files that will be used as an index. The file name can contain variables." } },
        { "try_files", new NginxDocItem { Syntax = "try_files file ... uri;", Description = "Checks the existence of files in the specified order and uses the first found file for request processing; the processing is performed in the current context." } },
        { "error_page", new NginxDocItem { Syntax = "error_page code ... [=[response]] uri;", Description = "Defines the URI that will be shown for the specified errors." } },
        { "recursive_error_pages", new NginxDocItem { Syntax = "recursive_error_pages on | off;", Description = "Enables or disables doing several redirects using the error_page directive." } },
        { "port_in_redirect", new NginxDocItem { Syntax = "port_in_redirect on | off;", Description = "Enables or disables specifying the port in absolute redirects issued by nginx." } },
        { "server_name_in_redirect", new NginxDocItem { Syntax = "server_name_in_redirect on | off;", Description = "Enables or disables the use of the primary server name, specified by the server_name directive, in absolute redirects issued by nginx." } },
        { "absolute_redirect", new NginxDocItem { Syntax = "absolute_redirect on | off;", Description = "If disabled, redirects issued by nginx will be relative." } },
        { "msie_padding", new NginxDocItem { Syntax = "msie_padding on | off;", Description = "Enables or disables adding comments to responses with status greater than 400 for MSIE clients to increase the response size to 512 bytes." } },
        { "msie_refresh", new NginxDocItem { Syntax = "msie_refresh on | off;", Description = "Enables or disables issuing a refresh instead of a redirect for MSIE clients." } },
        { "log_not_found", new NginxDocItem { Syntax = "log_not_found on | off;", Description = "Enables or disables logging of errors about not found files into error_log." } },
        { "keepalive_disable", new NginxDocItem { Syntax = "keepalive_disable none | browser ...;", Description = "Disables keep-alive connections with misbehaving browsers." } },
        { "keepalive_requests", new NginxDocItem { Syntax = "keepalive_requests number;", Description = "Sets the maximum number of requests that can be served through one keep-alive connection." } },
        { "keepalive_timeout", new NginxDocItem { Syntax = "keepalive_timeout timeout [header_timeout];", Description = "Sets a timeout during which a keep-alive client connection will stay open on the server side." } },
        { "types", new NginxDocItem { Syntax = "types { ... }", Description = "Maps file name extensions to MIME types of responses." } },
        { "default_type", new NginxDocItem { Syntax = "default_type mime-type;", Description = "Defines the default MIME type of a response." } },
        { "types_hash_max_size", new NginxDocItem { Syntax = "types_hash_max_size size;", Description = "Sets the maximum size of the types hash tables." } },
        { "types_hash_bucket_size", new NginxDocItem { Syntax = "types_hash_bucket_size size;", Description = "Sets the bucket size for the types hash tables." } },
        { "client_body_buffer_size", new NginxDocItem { Syntax = "client_body_buffer_size size;", Description = "Sets buffer size for reading client request body." } },
        { "client_body_in_file_only", new NginxDocItem { Syntax = "client_body_in_file_only on | clean | off;", Description = "Determines whether nginx should save the entire client request body into a file." } },
        { "client_body_in_single_buffer", new NginxDocItem { Syntax = "client_body_in_single_buffer on | off;", Description = "Determines whether to save the entire client request body in a single buffer." } },
        { "client_body_temp_path", new NginxDocItem { Syntax = "client_body_temp_path path [level1 [level2 [level3]]];", Description = "Defines a directory for storing temporary files holding client request bodies." } },
        { "client_body_timeout", new NginxDocItem { Syntax = "client_body_timeout time;", Description = "Defines a timeout for reading client request body." } },
        { "client_header_buffer_size", new NginxDocItem { Syntax = "client_header_buffer_size size;", Description = "Sets buffer size for reading the client request header." } },
        { "client_header_timeout", new NginxDocItem { Syntax = "client_header_timeout time;", Description = "Defines a timeout for reading client request header." } },
        { "client_max_body_size", new NginxDocItem { Syntax = "client_max_body_size size;", Description = "Sets the maximum allowed size of the client request body, specified in the 'Content-Length' request header field." } },
        { "large_client_header_buffers", new NginxDocItem { Syntax = "large_client_header_buffers number size;", Description = "Sets the maximum number and size of buffers used for reading large client request headers." } },
        { "ignore_invalid_headers", new NginxDocItem { Syntax = "ignore_invalid_headers on | off;", Description = "Controls whether invalid headers should be ignored." } },
        { "underscores_in_headers", new NginxDocItem { Syntax = "underscores_in_headers on | off;", Description = "Enables or disables the use of underscores in client request header fields." } },
        { "connection_pool_size", new NginxDocItem { Syntax = "connection_pool_size size;", Description = "Allows tuning the connection pool size." } },
        { "request_pool_size", new NginxDocItem { Syntax = "request_pool_size size;", Description = "Allows tuning the request pool size." } },
        { "if_modified_since", new NginxDocItem { Syntax = "if_modified_since off | exact | before;", Description = "Specifies how to compare the modification time of a response with the time in the 'If-Modified-Since' request header field." } },
        { "max_ranges", new NginxDocItem { Syntax = "max_ranges number;", Description = "Limits the maximum allowed number of ranges in byte-range requests." } },
        { "output_buffers", new NginxDocItem { Syntax = "output_buffers number size;", Description = "Sets the number and size of the buffers used for reading a response from a disk." } },
        { "postpone_output", new NginxDocItem { Syntax = "postpone_output size;", Description = "If possible, the transmission of client data will be postponed until nginx has at least size bytes of data to send." } },
        { "read_ahead", new NginxDocItem { Syntax = "read_ahead size;", Description = "Sets the amount of data that will be read from a file while working with the file kernel primitive." } },
        { "reset_timedout_connection", new NginxDocItem { Syntax = "reset_timedout_connection on | off;", Description = "Enables or disables resetting timed out connections." } },
        { "send_timeout", new NginxDocItem { Syntax = "send_timeout time;", Description = "Sets a timeout for transmitting a response to the client." } },
        { "sendfile", new NginxDocItem { Syntax = "sendfile on | off;", Description = "Enables or disables the use of sendfile()." } },
        { "sendfile_max_chunk", new NginxDocItem { Syntax = "sendfile_max_chunk size;", Description = "Limits the amount of data that can be transferred in a single sendfile() call." } },
        { "tcp_nodelay", new NginxDocItem { Syntax = "tcp_nodelay on | off;", Description = "Enables or disables the use of the TCP_NODELAY option." } },
        { "tcp_nopush", new NginxDocItem { Syntax = "tcp_nopush on | off;", Description = "Enables or disables the use of the TCP_CORK socket option on Linux, or TCP_NOPUSH on FreeBSD." } },
        { "server_name", new NginxDocItem { Syntax = "server_name name ...;", Description = "Sets names of a virtual server." } },
        { "server_names_hash_max_size", new NginxDocItem { Syntax = "server_names_hash_max_size size;", Description = "Sets the maximum size of the server names hash tables." } },
        { "server_names_hash_bucket_size", new NginxDocItem { Syntax = "server_names_hash_bucket_size size;", Description = "Sets the bucket size for the server names hash tables." } },
        { "listen", new NginxDocItem { Syntax = "listen address[:port] [default_server] [ssl] [http2];", Description = "Sets the address and port for IP, or the path for a UNIX-domain socket on which the server will accept requests." } },
        { "resolver", new NginxDocItem { Syntax = "resolver address ... [valid=time] [ipv6=on|off];", Description = "Configures name servers used to resolve names of upstream servers into addresses." } },
        { "resolver_timeout", new NginxDocItem { Syntax = "resolver_timeout time;", Description = "Sets a timeout for name resolution." } },
        { "satisfy", new NginxDocItem { Syntax = "satisfy all | any;", Description = "Allows access if all (all) or at least one (any) of the ngx_http_access_module, ngx_http_auth_basic_module, ngx_http_auth_request_module, or ngx_http_auth_jwt_module modules allow access." } },
        { "internal", new NginxDocItem { Syntax = "internal;", Description = "Specifies that a given location can only be used for internal requests." } },
        { "merge_slashes", new NginxDocItem { Syntax = "merge_slashes on | off;", Description = "Enables or disables compression of two or more adjacent slashes in a URI into a single slash." } },

        // --- Logging ---
        { "access_log", new NginxDocItem { Syntax = "access_log path [format [buffer=size]];", Description = "Sets the path, format, and configuration for a buffered log write." } },
        { "log_format", new NginxDocItem { Syntax = "log_format name string ...;", Description = "Specifies log format." } },
        { "open_log_file_cache", new NginxDocItem { Syntax = "open_log_file_cache max=N [inactive=time] [min_uses=N] [valid=time];", Description = "Defines a cache that stores the file descriptors of all frequently used log files whose names contain variables." } },

        // --- Gzip ---
        { "gzip", new NginxDocItem { Syntax = "gzip on | off;", Description = "Enables or disables gzipping of responses." } },
        { "gzip_buffers", new NginxDocItem { Syntax = "gzip_buffers number size;", Description = "Sets the number and size of buffers used to compress a response." } },
        { "gzip_comp_level", new NginxDocItem { Syntax = "gzip_comp_level level;", Description = "Sets a gzip compression level of a response. Acceptable values are in the range from 1 to 9." } },
        { "gzip_disable", new NginxDocItem { Syntax = "gzip_disable regex ...;", Description = "Disables gzipping of responses for requests with 'User-Agent' header fields matching any of the specified regular expressions." } },
        { "gzip_http_version", new NginxDocItem { Syntax = "gzip_http_version 1.0 | 1.1;", Description = "Sets the minimum HTTP version of a request required to compress a response." } },
        { "gzip_min_length", new NginxDocItem { Syntax = "gzip_min_length length;", Description = "Sets the minimum length of a response that will be gzipped." } },
        { "gzip_proxied", new NginxDocItem { Syntax = "gzip_proxied off | expired | no-cache | ...;", Description = "Enables or disables gzipping of responses for proxied requests depending on the request and response." } },
        { "gzip_types", new NginxDocItem { Syntax = "gzip_types mime-type ...;", Description = "Enables gzipping of responses for the specified MIME types in addition to 'text/html'." } },
        { "gzip_vary", new NginxDocItem { Syntax = "gzip_vary on | off;", Description = "Enables or disables inserting the 'Vary: Accept-Encoding' response header field." } },
        { "gzip_static", new NginxDocItem { Syntax = "gzip_static on | off | always;", Description = "Enables checking for the existence of precompressed files with the same name as the original file and a '.gz' extension." } },
        { "gunzip", new NginxDocItem { Syntax = "gunzip on | off;", Description = "Enables or disables decompression of gzipped responses for clients that do not support 'gzip' encoding method." } },
        { "gunzip_buffers", new NginxDocItem { Syntax = "gunzip_buffers number size;", Description = "Sets the number and size of buffers used to decompress a response." } },

        // --- SSL / TLS ---
        { "ssl", new NginxDocItem { Syntax = "ssl on | off;", Description = "Enables the HTTPS protocol for the given virtual server." } },
        { "ssl_buffer_size", new NginxDocItem { Syntax = "ssl_buffer_size size;", Description = "Sets the size of the buffer used for sending data." } },
        { "ssl_certificate", new NginxDocItem { Syntax = "ssl_certificate file;", Description = "Specifies a file with the certificate in the PEM format for the given virtual server." } },
        { "ssl_certificate_key", new NginxDocItem { Syntax = "ssl_certificate_key file;", Description = "Specifies a file with the secret key in the PEM format for the given virtual server." } },
        { "ssl_ciphers", new NginxDocItem { Syntax = "ssl_ciphers ciphers;", Description = "Specifies the enabled ciphers. The ciphers are specified in the format understood by the OpenSSL library." } },
        { "ssl_client_certificate", new NginxDocItem { Syntax = "ssl_client_certificate file;", Description = "Specifies a file with trusted CA certificates in the PEM format used to verify client certificates." } },
        { "ssl_crl", new NginxDocItem { Syntax = "ssl_crl file;", Description = "Specifies a file with revoked certificates (CRL) in the PEM format." } },
        { "ssl_dhparam", new NginxDocItem { Syntax = "ssl_dhparam file;", Description = "Specifies a file with DH parameters for DHE ciphers." } },
        { "ssl_ecdh_curve", new NginxDocItem { Syntax = "ssl_ecdh_curve curve;", Description = "Specifies a curve for ECDHE ciphers." } },
        { "ssl_password_file", new NginxDocItem { Syntax = "ssl_password_file file;", Description = "Specifies a file with passphrases for secret keys, where each passphrase is specified on a separate line." } },
        { "ssl_prefer_server_ciphers", new NginxDocItem { Syntax = "ssl_prefer_server_ciphers on | off;", Description = "Specifies that server ciphers should be preferred over client ciphers when using the SSLv3 and TLS protocols." } },
        { "ssl_protocols", new NginxDocItem { Syntax = "ssl_protocols protocol ...;", Description = "Enables the specified protocols (TLSv1, TLSv1.1, TLSv1.2, TLSv1.3)." } },
        { "ssl_session_cache", new NginxDocItem { Syntax = "ssl_session_cache off | none | [builtin[:size]] [shared:name:size];", Description = "Sets the types and sizes of caches that store session parameters." } },
        { "ssl_session_ticket_key", new NginxDocItem { Syntax = "ssl_session_ticket_key file;", Description = "Sets a file with the secret key used to encrypt and decrypt TLS session tickets." } },
        { "ssl_session_tickets", new NginxDocItem { Syntax = "ssl_session_tickets on | off;", Description = "Enables or disables session resumption through TLS session tickets." } },
        { "ssl_session_timeout", new NginxDocItem { Syntax = "ssl_session_timeout time;", Description = "Specifies a time during which the client may reuse the session parameters." } },
        { "ssl_stapling", new NginxDocItem { Syntax = "ssl_stapling on | off;", Description = "Enables or disables OCSP Stapling." } },
        { "ssl_stapling_file", new NginxDocItem { Syntax = "ssl_stapling_file file;", Description = "When the ssl_stapling directive is set, this directive specifies a file with the OCSP response." } },
        { "ssl_stapling_responder", new NginxDocItem { Syntax = "ssl_stapling_responder url;", Description = "Overrides the URL of the OCSP responder." } },
        { "ssl_stapling_verify", new NginxDocItem { Syntax = "ssl_stapling_verify on | off;", Description = "Enables or disables verification of OCSP responses by the server." } },
        { "ssl_trusted_certificate", new NginxDocItem { Syntax = "ssl_trusted_certificate file;", Description = "Specifies a file with trusted CA certificates in the PEM format used to verify the OCSP response." } },
        { "ssl_verify_client", new NginxDocItem { Syntax = "ssl_verify_client on | off | optional | optional_no_ca;", Description = "Enables verification of client certificates." } },
        { "ssl_verify_depth", new NginxDocItem { Syntax = "ssl_verify_depth number;", Description = "Sets the verification depth in the client certificate chain." } },

        // --- Proxy ---
        { "proxy_pass", new NginxDocItem { Syntax = "proxy_pass URL;", Description = "Sets the protocol and address of a proxied server and an optional URI to which a location should be mapped." } },
        { "proxy_bind", new NginxDocItem { Syntax = "proxy_bind address [transparent] | off;", Description = "Makes outgoing connections to a proxied server originate from the specified local IP address." } },
        { "proxy_buffer_size", new NginxDocItem { Syntax = "proxy_buffer_size size;", Description = "Sets the size of the buffer used for reading the first part of the response received from the proxied server." } },
        { "proxy_buffering", new NginxDocItem { Syntax = "proxy_buffering on | off;", Description = "Enables or disables buffering of responses from the proxied server." } },
        { "proxy_buffers", new NginxDocItem { Syntax = "proxy_buffers number size;", Description = "Sets the number and size of the buffers used for reading a response from the proxied server." } },
        { "proxy_busy_buffers_size", new NginxDocItem { Syntax = "proxy_busy_buffers_size size;", Description = "Limiting the total size of buffers that can be busy sending a response to the client while the response is not yet fully read." } },
        { "proxy_cache", new NginxDocItem { Syntax = "proxy_cache zone | off;", Description = "Defines a shared memory zone used for caching." } },
        { "proxy_cache_background_update", new NginxDocItem { Syntax = "proxy_cache_background_update on | off;", Description = "Allows starting a background subrequest to update an expired cache item." } },
        { "proxy_cache_bypass", new NginxDocItem { Syntax = "proxy_cache_bypass string ...;", Description = "Defines conditions under which the response will not be taken from a cache." } },
        { "proxy_cache_convert_head", new NginxDocItem { Syntax = "proxy_cache_convert_head on | off;", Description = "Enables or disables the conversion of the 'HEAD' method to 'GET' for caching." } },
        { "proxy_cache_key", new NginxDocItem { Syntax = "proxy_cache_key string;", Description = "Defines a key for caching, for example: '$scheme$proxy_host$uri$is_args$args'." } },
        { "proxy_cache_lock", new NginxDocItem { Syntax = "proxy_cache_lock on | off;", Description = "When enabled, only one request at a time will be allowed to populate a new cache element." } },
        { "proxy_cache_lock_age", new NginxDocItem { Syntax = "proxy_cache_lock_age time;", Description = "If the last request passed to the proxied server for populating a new cache element has not completed for the specified time, one more request may be passed." } },
        { "proxy_cache_lock_timeout", new NginxDocItem { Syntax = "proxy_cache_lock_timeout time;", Description = "Sets a timeout for proxy_cache_lock." } },
        { "proxy_cache_max_range_offset", new NginxDocItem { Syntax = "proxy_cache_max_range_offset number;", Description = "Sets the offset in bytes for byte-range requests." } },
        { "proxy_cache_methods", new NginxDocItem { Syntax = "proxy_cache_methods GET | HEAD | POST ...;", Description = "If the client request method is listed in this directive then the response will be cached." } },
        { "proxy_cache_min_uses", new NginxDocItem { Syntax = "proxy_cache_min_uses number;", Description = "Sets the number of requests after which the response will be cached." } },
        { "proxy_cache_path", new NginxDocItem { Syntax = "proxy_cache_path path [levels=levels] ...;", Description = "Sets the path and other parameters of a cache." } },
        { "proxy_cache_purge", new NginxDocItem { Syntax = "proxy_cache_purge string ...;", Description = "Defines conditions under which the request will be considered a cache purge request." } },
        { "proxy_cache_revalidate", new NginxDocItem { Syntax = "proxy_cache_revalidate on | off;", Description = "Enables revalidation of expired cache items using conditional requests with the 'If-Modified-Since' and 'If-None-Match' header fields." } },
        { "proxy_cache_use_stale", new NginxDocItem { Syntax = "proxy_cache_use_stale error | timeout | ...;", Description = "Determines in which cases a stale cached response can be used when an error occurs during communication with the proxied server." } },
        { "proxy_cache_valid", new NginxDocItem { Syntax = "proxy_cache_valid [code ...] time;", Description = "Sets caching time for different response codes." } },
        { "proxy_connect_timeout", new NginxDocItem { Syntax = "proxy_connect_timeout time;", Description = "Defines a timeout for establishing a connection with a proxied server." } },
        { "proxy_cookie_domain", new NginxDocItem { Syntax = "proxy_cookie_domain domain replacement;", Description = "Sets a text that should be changed in the 'domain' attribute of the 'Set-Cookie' header fields of a proxied server response." } },
        { "proxy_cookie_path", new NginxDocItem { Syntax = "proxy_cookie_path path replacement;", Description = "Sets a text that should be changed in the 'path' attribute of the 'Set-Cookie' header fields of a proxied server response." } },
        { "proxy_force_ranges", new NginxDocItem { Syntax = "proxy_force_ranges on | off;", Description = "Enables byte-range support for both cached and uncached responses from the proxied server." } },
        { "proxy_headers_hash_bucket_size", new NginxDocItem { Syntax = "proxy_headers_hash_bucket_size size;", Description = "Sets the bucket size for the proxy_headers_hash tables." } },
        { "proxy_headers_hash_max_size", new NginxDocItem { Syntax = "proxy_headers_hash_max_size size;", Description = "Sets the maximum size of the proxy_headers_hash tables." } },
        { "proxy_hide_header", new NginxDocItem { Syntax = "proxy_hide_header field;", Description = "Sets additional fields that will not be passed to the client in the response." } },
        { "proxy_http_version", new NginxDocItem { Syntax = "proxy_http_version 1.0 | 1.1;", Description = "Sets the HTTP protocol version for proxying." } },
        { "proxy_ignore_client_abort", new NginxDocItem { Syntax = "proxy_ignore_client_abort on | off;", Description = "Determines whether the connection with a proxied server should be closed when a client closes the connection without waiting for a response." } },
        { "proxy_ignore_headers", new NginxDocItem { Syntax = "proxy_ignore_headers field ...;", Description = "Disables processing of certain response header fields from the proxied server." } },
        { "proxy_intercept_errors", new NginxDocItem { Syntax = "proxy_intercept_errors on | off;", Description = "Determines whether proxied responses with codes greater than or equal to 300 should be passed to a client or be intercepted and redirected to nginx for processing with the error_page directive." } },
        { "proxy_limit_rate", new NginxDocItem { Syntax = "proxy_limit_rate rate;", Description = "Limits the speed of reading the response from the proxied server." } },
        { "proxy_max_temp_file_size", new NginxDocItem { Syntax = "proxy_max_temp_file_size size;", Description = "When buffering of responses from the proxied server is enabled, limits the total size of temporary files." } },
        { "proxy_method", new NginxDocItem { Syntax = "proxy_method method;", Description = "Specifies the HTTP method to use in requests forwarded to the proxied server." } },
        { "proxy_next_upstream", new NginxDocItem { Syntax = "proxy_next_upstream error | timeout | ...;", Description = "Specifies in which cases a request should be passed to the next server." } },
        { "proxy_next_upstream_timeout", new NginxDocItem { Syntax = "proxy_next_upstream_timeout time;", Description = "Limits the time during which a request can be passed to the next server." } },
        { "proxy_next_upstream_tries", new NginxDocItem { Syntax = "proxy_next_upstream_tries number;", Description = "Limits the number of possible tries for passing a request to the next server." } },
        { "proxy_no_cache", new NginxDocItem { Syntax = "proxy_no_cache string ...;", Description = "Defines conditions under which the response will not be saved to a cache." } },
        { "proxy_pass_header", new NginxDocItem { Syntax = "proxy_pass_header field;", Description = "Permits passing otherwise disabled header fields." } },
        { "proxy_pass_request_body", new NginxDocItem { Syntax = "proxy_pass_request_body on | off;", Description = "Indicates whether the original request body is passed to the proxied server." } },
        { "proxy_pass_request_headers", new NginxDocItem { Syntax = "proxy_pass_request_headers on | off;", Description = "Indicates whether the header fields of the original request are passed to the proxied server." } },
        { "proxy_read_timeout", new NginxDocItem { Syntax = "proxy_read_timeout time;", Description = "Defines a timeout for reading a response from the proxied server." } },
        { "proxy_redirect", new NginxDocItem { Syntax = "proxy_redirect default | off | replacement;", Description = "Sets the text that should be changed in the 'Location' and 'Refresh' header fields of a proxied server response." } },
        { "proxy_request_buffering", new NginxDocItem { Syntax = "proxy_request_buffering on | off;", Description = "Enables or disables buffering of a client request body." } },
        { "proxy_send_lowat", new NginxDocItem { Syntax = "proxy_send_lowat size;", Description = "If the directive is set to a non-zero value, nginx will try to minimize the number of send operations to the proxied server." } },
        { "proxy_send_timeout", new NginxDocItem { Syntax = "proxy_send_timeout time;", Description = "Sets a timeout for transmitting a request to the proxied server." } },
        { "proxy_set_body", new NginxDocItem { Syntax = "proxy_set_body value;", Description = "Allows redefining the request body passed to the proxied server." } },
        { "proxy_set_header", new NginxDocItem { Syntax = "proxy_set_header field value;", Description = "Allows redefining or appending fields to the request header passed to the proxied server." } },
        { "proxy_ssl_certificate", new NginxDocItem { Syntax = "proxy_ssl_certificate file;", Description = "Specifies a file with the certificate in the PEM format used for authentication to a proxied HTTPS server." } },
        { "proxy_ssl_certificate_key", new NginxDocItem { Syntax = "proxy_ssl_certificate_key file;", Description = "Specifies a file with the secret key in the PEM format used for authentication to a proxied HTTPS server." } },
        { "proxy_ssl_ciphers", new NginxDocItem { Syntax = "proxy_ssl_ciphers ciphers;", Description = "Specifies the enabled ciphers for requests to a proxied HTTPS server." } },
        { "proxy_ssl_crl", new NginxDocItem { Syntax = "proxy_ssl_crl file;", Description = "Specifies a file with revoked certificates (CRL) in the PEM format used to verify the certificate of the proxied HTTPS server." } },
        { "proxy_ssl_name", new NginxDocItem { Syntax = "proxy_ssl_name name;", Description = "Allows overriding the server name used to verify the certificate of the proxied HTTPS server and to be passed through SNI." } },
        { "proxy_ssl_password_file", new NginxDocItem { Syntax = "proxy_ssl_password_file file;", Description = "Specifies a file with passphrases for secret keys used for authentication to a proxied HTTPS server." } },
        { "proxy_ssl_server_name", new NginxDocItem { Syntax = "proxy_ssl_server_name on | off;", Description = "Enables or disables passing of the server name through the TLS Server Name Indication extension (SNI) when establishing a connection with the proxied HTTPS server." } },
        { "proxy_ssl_session_reuse", new NginxDocItem { Syntax = "proxy_ssl_session_reuse on | off;", Description = "Determines whether TLS session reuse is enabled when working with the proxied server." } },
        { "proxy_ssl_protocols", new NginxDocItem { Syntax = "proxy_ssl_protocols protocol ...;", Description = "Enables the specified protocols for requests to a proxied HTTPS server." } },
        { "proxy_ssl_trusted_certificate", new NginxDocItem { Syntax = "proxy_ssl_trusted_certificate file;", Description = "Specifies a file with trusted CA certificates in the PEM format used to verify the certificate of the proxied HTTPS server." } },
        { "proxy_ssl_verify", new NginxDocItem { Syntax = "proxy_ssl_verify on | off;", Description = "Enables or disables verification of the proxied HTTPS server certificate." } },
        { "proxy_ssl_verify_depth", new NginxDocItem { Syntax = "proxy_ssl_verify_depth number;", Description = "Sets the verification depth in the proxied HTTPS server certificate chain." } },
        { "proxy_store", new NginxDocItem { Syntax = "proxy_store on | off | string;", Description = "Enables saving of files to a disk." } },
        { "proxy_store_access", new NginxDocItem { Syntax = "proxy_store_access users:permissions ...;", Description = "Sets access permissions for newly created files and directories." } },
        { "proxy_temp_file_write_size", new NginxDocItem { Syntax = "proxy_temp_file_write_size size;", Description = "Limits the size of data written to a temporary file at a time." } },
        { "proxy_temp_path", new NginxDocItem { Syntax = "proxy_temp_path path [level1 [level2 [level3]]];", Description = "Defines a directory for storing temporary files with data received from proxied servers." } },

        // --- FastCGI (PHP, etc) ---
        { "fastcgi_pass", new NginxDocItem { Syntax = "fastcgi_pass address;", Description = "Sets the address of a FastCGI server." } },
        { "fastcgi_bind", new NginxDocItem { Syntax = "fastcgi_bind address [transparent] | off;", Description = "Makes outgoing connections to a FastCGI server originate from the specified local IP address." } },
        { "fastcgi_buffer_size", new NginxDocItem { Syntax = "fastcgi_buffer_size size;", Description = "Sets the size of the buffer used for reading the first part of the response received from the FastCGI server." } },
        { "fastcgi_buffering", new NginxDocItem { Syntax = "fastcgi_buffering on | off;", Description = "Enables or disables buffering of responses from the FastCGI server." } },
        { "fastcgi_buffers", new NginxDocItem { Syntax = "fastcgi_buffers number size;", Description = "Sets the number and size of the buffers used for reading a response from the FastCGI server." } },
        { "fastcgi_busy_buffers_size", new NginxDocItem { Syntax = "fastcgi_busy_buffers_size size;", Description = "Limiting the total size of buffers that can be busy sending a response to the client while the response is not yet fully read." } },
        { "fastcgi_cache", new NginxDocItem { Syntax = "fastcgi_cache zone | off;", Description = "Defines a shared memory zone used for caching." } },
        { "fastcgi_cache_background_update", new NginxDocItem { Syntax = "fastcgi_cache_background_update on | off;", Description = "Allows starting a background subrequest to update an expired cache item." } },
        { "fastcgi_cache_bypass", new NginxDocItem { Syntax = "fastcgi_cache_bypass string ...;", Description = "Defines conditions under which the response will not be taken from a cache." } },
        { "fastcgi_cache_key", new NginxDocItem { Syntax = "fastcgi_cache_key string;", Description = "Defines a key for caching." } },
        { "fastcgi_cache_lock", new NginxDocItem { Syntax = "fastcgi_cache_lock on | off;", Description = "When enabled, only one request at a time will be allowed to populate a new cache element." } },
        { "fastcgi_cache_lock_age", new NginxDocItem { Syntax = "fastcgi_cache_lock_age time;", Description = "If the last request passed to the FastCGI server for populating a new cache element has not completed for the specified time, one more request may be passed." } },
        { "fastcgi_cache_lock_timeout", new NginxDocItem { Syntax = "fastcgi_cache_lock_timeout time;", Description = "Sets a timeout for fastcgi_cache_lock." } },
        { "fastcgi_cache_max_range_offset", new NginxDocItem { Syntax = "fastcgi_cache_max_range_offset number;", Description = "Sets the offset in bytes for byte-range requests." } },
        { "fastcgi_cache_methods", new NginxDocItem { Syntax = "fastcgi_cache_methods GET | HEAD | POST ...;", Description = "If the client request method is listed in this directive then the response will be cached." } },
        { "fastcgi_cache_min_uses", new NginxDocItem { Syntax = "fastcgi_cache_min_uses number;", Description = "Sets the number of requests after which the response will be cached." } },
        { "fastcgi_cache_path", new NginxDocItem { Syntax = "fastcgi_cache_path path [levels=levels] ...;", Description = "Sets the path and other parameters of a cache." } },
        { "fastcgi_cache_purge", new NginxDocItem { Syntax = "fastcgi_cache_purge string ...;", Description = "Defines conditions under which the request will be considered a cache purge request." } },
        { "fastcgi_cache_revalidate", new NginxDocItem { Syntax = "fastcgi_cache_revalidate on | off;", Description = "Enables revalidation of expired cache items using conditional requests." } },
        { "fastcgi_cache_use_stale", new NginxDocItem { Syntax = "fastcgi_cache_use_stale error | timeout | ...;", Description = "Determines in which cases a stale cached response can be used." } },
        { "fastcgi_cache_valid", new NginxDocItem { Syntax = "fastcgi_cache_valid [code ...] time;", Description = "Sets caching time for different response codes." } },
        { "fastcgi_catch_stderr", new NginxDocItem { Syntax = "fastcgi_catch_stderr string;", Description = "Sets a string to search for in the error stream of a response received from the FastCGI server." } },
        { "fastcgi_connect_timeout", new NginxDocItem { Syntax = "fastcgi_connect_timeout time;", Description = "Defines a timeout for establishing a connection with a FastCGI server." } },
        { "fastcgi_force_ranges", new NginxDocItem { Syntax = "fastcgi_force_ranges on | off;", Description = "Enables byte-range support for both cached and uncached responses from the FastCGI server." } },
        { "fastcgi_hide_header", new NginxDocItem { Syntax = "fastcgi_hide_header field;", Description = "Sets additional fields that will not be passed to the client in the response." } },
        { "fastcgi_ignore_client_abort", new NginxDocItem { Syntax = "fastcgi_ignore_client_abort on | off;", Description = "Determines whether the connection with a FastCGI server should be closed when a client closes the connection without waiting for a response." } },
        { "fastcgi_ignore_headers", new NginxDocItem { Syntax = "fastcgi_ignore_headers field ...;", Description = "Disables processing of certain response header fields from the FastCGI server." } },
        { "fastcgi_index", new NginxDocItem { Syntax = "fastcgi_index name;", Description = "Sets a file name that will be appended after a URI ending with a slash." } },
        { "fastcgi_intercept_errors", new NginxDocItem { Syntax = "fastcgi_intercept_errors on | off;", Description = "Determines whether FastCGI responses with codes greater than or equal to 300 should be passed to a client or be intercepted." } },
        { "fastcgi_keep_conn", new NginxDocItem { Syntax = "fastcgi_keep_conn on | off;", Description = "By default, a FastCGI server will close a connection right after sending the response. However, if this directive is set to on, nginx will instruct a FastCGI server to keep the connection open." } },
        { "fastcgi_limit_rate", new NginxDocItem { Syntax = "fastcgi_limit_rate rate;", Description = "Limits the speed of reading the response from the FastCGI server." } },
        { "fastcgi_max_temp_file_size", new NginxDocItem { Syntax = "fastcgi_max_temp_file_size size;", Description = "When buffering of responses from the FastCGI server is enabled, limits the total size of temporary files." } },
        { "fastcgi_next_upstream", new NginxDocItem { Syntax = "fastcgi_next_upstream error | timeout | ...;", Description = "Specifies in which cases a request should be passed to the next server." } },
        { "fastcgi_next_upstream_timeout", new NginxDocItem { Syntax = "fastcgi_next_upstream_timeout time;", Description = "Limits the time during which a request can be passed to the next server." } },
        { "fastcgi_next_upstream_tries", new NginxDocItem { Syntax = "fastcgi_next_upstream_tries number;", Description = "Limits the number of possible tries for passing a request to the next server." } },
        { "fastcgi_no_cache", new NginxDocItem { Syntax = "fastcgi_no_cache string ...;", Description = "Defines conditions under which the response will not be saved to a cache." } },
        { "fastcgi_param", new NginxDocItem { Syntax = "fastcgi_param parameter value [if_not_empty];", Description = "Sets a parameter that should be passed to the FastCGI server." } },
        { "fastcgi_pass_header", new NginxDocItem { Syntax = "fastcgi_pass_header field;", Description = "Permits passing otherwise disabled header fields." } },
        { "fastcgi_pass_request_body", new NginxDocItem { Syntax = "fastcgi_pass_request_body on | off;", Description = "Indicates whether the original request body is passed to the FastCGI server." } },
        { "fastcgi_pass_request_headers", new NginxDocItem { Syntax = "fastcgi_pass_request_headers on | off;", Description = "Indicates whether the header fields of the original request are passed to the FastCGI server." } },
        { "fastcgi_read_timeout", new NginxDocItem { Syntax = "fastcgi_read_timeout time;", Description = "Defines a timeout for reading a response from the FastCGI server." } },
        { "fastcgi_request_buffering", new NginxDocItem { Syntax = "fastcgi_request_buffering on | off;", Description = "Enables or disables buffering of a client request body." } },
        { "fastcgi_send_lowat", new NginxDocItem { Syntax = "fastcgi_send_lowat size;", Description = "If the directive is set to a non-zero value, nginx will try to minimize the number of send operations to the FastCGI server." } },
        { "fastcgi_send_timeout", new NginxDocItem { Syntax = "fastcgi_send_timeout time;", Description = "Sets a timeout for transmitting a request to the FastCGI server." } },
        { "fastcgi_split_path_info", new NginxDocItem { Syntax = "fastcgi_split_path_info regex;", Description = "Defines a regular expression that captures a value for the $fastcgi_path_info variable." } },
        { "fastcgi_store", new NginxDocItem { Syntax = "fastcgi_store on | off | string;", Description = "Enables saving of files to a disk." } },
        { "fastcgi_store_access", new NginxDocItem { Syntax = "fastcgi_store_access users:permissions ...;", Description = "Sets access permissions for newly created files and directories." } },
        { "fastcgi_temp_file_write_size", new NginxDocItem { Syntax = "fastcgi_temp_file_write_size size;", Description = "Limits the size of data written to a temporary file at a time." } },
        { "fastcgi_temp_path", new NginxDocItem { Syntax = "fastcgi_temp_path path [level1 [level2 [level3]]];", Description = "Defines a directory for storing temporary files with data received from FastCGI servers." } },

        // --- Upstream ---
        { "upstream", new NginxDocItem { Syntax = "upstream name { ... }", Description = "Defines a group of servers. Servers can listen on different ports. In addition, servers listening on TCP and UNIX-domain sockets can be mixed." } },
        { "ip_hash", new NginxDocItem { Syntax = "ip_hash;", Description = "Specifies that a group should use a load balancing method where requests are distributed between servers based on client IP addresses." } },
        { "least_conn", new NginxDocItem { Syntax = "least_conn;", Description = "Specifies that a group should use a load balancing method where a request is passed to the server with the least number of active connections." } },
        { "least_time", new NginxDocItem { Syntax = "least_time header | last_byte [inflight];", Description = "Specifies that a group should use a load balancing method where a request is passed to the server with the least average response time and least number of active connections." } },
        { "keepalive", new NginxDocItem { Syntax = "keepalive connections;", Description = "Activates the cache for connections to upstream servers." } },
        { "ntlm", new NginxDocItem { Syntax = "ntlm;", Description = "Allows NTLM authentication with proxied servers." } },
        { "sticky", new NginxDocItem { Syntax = "sticky cookie name ...;", Description = "Enables session affinity, which causes requests from the same client to be passed to the same server in a group." } },
        { "zone", new NginxDocItem { Syntax = "zone name [size];", Description = "Defines the shared memory zone that stores the group's configuration and run-time state." } },
        { "hash", new NginxDocItem { Syntax = "hash key [consistent];", Description = "Specifies a load balancing method for a server group where the client-server mapping is based on the hashed key value." } },
        { "random", new NginxDocItem { Syntax = "random [two [method]];", Description = "Specifies a load balancing method for a server group where a request is passed to a randomly selected server." } },
        { "queue", new NginxDocItem { Syntax = "queue number [timeout=time];", Description = "If an upstream server cannot be selected immediately while processing a request, the request will be placed into the queue." } },

        // --- Access & Auth ---
        { "allow", new NginxDocItem { Syntax = "allow address | CIDR | unix: | all;", Description = "Allows access for the specified network or address." } },
        { "deny", new NginxDocItem { Syntax = "deny address | CIDR | unix: | all;", Description = "Denies access for the specified network or address." } },
        { "auth_basic", new NginxDocItem { Syntax = "auth_basic string | off;", Description = "Enables validation of user name and password using the \"HTTP Basic Authentication\" protocol." } },
        { "auth_basic_user_file", new NginxDocItem { Syntax = "auth_basic_user_file file;", Description = "Specifies a file that keeps user names and passwords." } },
        { "auth_request", new NginxDocItem { Syntax = "auth_request uri | off;", Description = "Enables authorization based on the result of a subrequest and sets the URI to which the subrequest will be sent." } },
        { "auth_request_set", new NginxDocItem { Syntax = "auth_request_set $variable value;", Description = "Sets the request variable to the given value after the auth subrequest completes." } },

        // --- Headers & Expires ---
        { "add_header", new NginxDocItem { Syntax = "add_header name value [always];", Description = "Adds the specified field to a response header provided that the response code equals 200, 201, 204, 206, 301, 302, 303, 304, 307, or 308." } },
        { "expires", new NginxDocItem { Syntax = "expires [modified] time;", Description = "Enables or disables adding or modifying the \"Expires\" and \"Cache-Control\" response header fields." } },
        { "add_trailer", new NginxDocItem { Syntax = "add_trailer name value [always];", Description = "Adds the specified field to the end of a response trailer provided that the response code equals 200, 201, 206, 301, 302, 303, 307, or 308." } },

        // --- Rewrites & Logic ---
        { "rewrite", new NginxDocItem { Syntax = "rewrite regex replacement [flag];", Description = "If the specified regular expression matches a request URI, URI is changed as specified in the replacement string." } },
        { "return", new NginxDocItem { Syntax = "return code [text] | code URL | URL;", Description = "Stops processing and returns the specified code to a client." } },
        { "break", new NginxDocItem { Syntax = "break;", Description = "Stops processing the current set of ngx_http_rewrite_module directives." } },
        { "if", new NginxDocItem { Syntax = "if (condition) { ... }", Description = "Checks a condition. If true, directives inside the braces are executed." } },
        { "set", new NginxDocItem { Syntax = "set $variable value;", Description = "Sets a value for the specified variable." } },
        { "uninitialized_variable_warn", new NginxDocItem { Syntax = "uninitialized_variable_warn on | off;", Description = "Controls whether or not warnings about uninitialized variables are logged." } },
        { "rewrite_log", new NginxDocItem { Syntax = "rewrite_log on | off;", Description = "Enables or disables logging of ngx_http_rewrite_module processing results into the error_log at notice level." } },
        { "valid_referers", new NginxDocItem { Syntax = "valid_referers none | blocked | server_names | string ...;", Description = "Specifies the \"Referer\" request header field values that will cause the embedded $invalid_referer variable to be set to an empty string." } },

        // --- Real IP ---
        { "set_real_ip_from", new NginxDocItem { Syntax = "set_real_ip_from address | CIDR | unix:;", Description = "Defines trusted addresses that are known to send correct replacement addresses." } },
        { "real_ip_header", new NginxDocItem { Syntax = "real_ip_header field | X-Real-IP | X-Forwarded-For | proxy_protocol;", Description = "Defines the request header field whose value will be used to replace the client address." } },
        { "real_ip_recursive", new NginxDocItem { Syntax = "real_ip_recursive on | off;", Description = "If recursive search is disabled, the original client address that matches one of the trusted addresses is replaced by the last address sent in the request header field defined by the real_ip_header directive." } },

        // --- Limits ---
        { "limit_conn", new NginxDocItem { Syntax = "limit_conn zone number;", Description = "Sets the shared memory zone and the maximum allowed number of connections for a given key value." } },
        { "limit_conn_log_level", new NginxDocItem { Syntax = "limit_conn_log_level info | notice | warn | error;", Description = "Sets the desired logging level for cases when the server limits the number of connections." } },
        { "limit_conn_status", new NginxDocItem { Syntax = "limit_conn_status code;", Description = "Sets the status code to return in response to rejected connections." } },
        { "limit_conn_zone", new NginxDocItem { Syntax = "limit_conn_zone key zone=name:size;", Description = "Sets parameters for a shared memory zone that will keep states for various keys." } },
        { "limit_rate", new NginxDocItem { Syntax = "limit_rate rate;", Description = "Limits the rate of response transmission to a client." } },
        { "limit_rate_after", new NginxDocItem { Syntax = "limit_rate_after size;", Description = "Sets the initial amount after which the further transmission of a response to a client will be rate limited." } },
        { "limit_req", new NginxDocItem { Syntax = "limit_req zone=name [burst=number] [nodelay | delay=number];", Description = "Sets the shared memory zone and the maximum burst size of requests." } },
        { "limit_req_log_level", new NginxDocItem { Syntax = "limit_req_log_level info | notice | warn | error;", Description = "Sets the desired logging level for cases when the server limits the processing rate of requests." } },
        { "limit_req_status", new NginxDocItem { Syntax = "limit_req_status code;", Description = "Sets the status code to return in response to rejected requests." } },
        { "limit_req_zone", new NginxDocItem { Syntax = "limit_req_zone key zone=name:size rate=rate [sync];", Description = "Sets parameters for a shared memory zone that will keep states for various keys." } },

        // --- Map & Split ---
        { "map", new NginxDocItem { Syntax = "map string $variable { ... }", Description = "Creates a new variable whose value depends on values of one or more of the source variables." } },
        { "map_hash_bucket_size", new NginxDocItem { Syntax = "map_hash_bucket_size size;", Description = "Sets the bucket size for the map variables hash tables." } },
        { "map_hash_max_size", new NginxDocItem { Syntax = "map_hash_max_size size;", Description = "Sets the maximum size of the map variables hash tables." } },
        { "split_clients", new NginxDocItem { Syntax = "split_clients string $variable { ... }", Description = "Creates a variable for A/B testing, for example: split_clients \"${remote_addr}AAA\" $variant { ... }" } },

        // --- Charset ---
        { "charset", new NginxDocItem { Syntax = "charset charset | off;", Description = "Adds the specified charset to the \"Content-Type\" response header field." } },
        { "charset_map", new NginxDocItem { Syntax = "charset_map charset1 charset2 { ... }", Description = "Describes the conversion table from one character set to another." } },
        { "charset_types", new NginxDocItem { Syntax = "charset_types mime-type ...;", Description = "Enables module processing in responses with the specified MIME types in addition to \"text/html\"." } },
        { "override_charset", new NginxDocItem { Syntax = "override_charset on | off;", Description = "Determines whether a conversion should be performed for answers received from a proxied server or from a FastCGI/uwsgi/SCGI/gRPC server." } },
        { "source_charset", new NginxDocItem { Syntax = "source_charset charset;", Description = "Defines the source charset of a response." } },

        // --- Auto Index ---
        { "autoindex", new NginxDocItem { Syntax = "autoindex on | off;", Description = "Enables or disables the directory listing output." } },
        { "autoindex_exact_size", new NginxDocItem { Syntax = "autoindex_exact_size on | off;", Description = "Specifies whether to output exact file sizes in the directory listing or round to KB, MB, or GB." } },
        { "autoindex_format", new NginxDocItem { Syntax = "autoindex_format html | xml | json | jsonp;", Description = "Sets the format of a directory listing." } },
        { "autoindex_localtime", new NginxDocItem { Syntax = "autoindex_localtime on | off;", Description = "Specifies whether to output file times in the directory listing in local time or UTC." } },

        // --- Browser ---
        { "ancient_browser", new NginxDocItem { Syntax = "ancient_browser string ...;", Description = "If any of the specified substrings is found in the \"User-Agent\" request header field, the $ancient_browser variable is set to \"1\"." } },
        { "ancient_browser_value", new NginxDocItem { Syntax = "ancient_browser_value string;", Description = "Sets the value for the $ancient_browser_value variable." } },
        { "modern_browser", new NginxDocItem { Syntax = "modern_browser browser_name version ...;", Description = "Specifies a version starting from which a browser is considered modern." } },
        { "modern_browser_value", new NginxDocItem { Syntax = "modern_browser_value string;", Description = "Sets the value for the $modern_browser_value variable." } },

        // --- HTTP Variables ---
        { "$arg_", new NginxDocItem { Syntax = "$arg_name", Description = "Argument in the request line." } },
        { "$args", new NginxDocItem { Syntax = "$args", Description = "Arguments in the request line." } },
        { "$binary_remote_addr", new NginxDocItem { Syntax = "$binary_remote_addr", Description = "Client address in a binary form, value's length is always 4 bytes for IPv4 addresses or 16 bytes for IPv6 addresses." } },
        { "$body_bytes_sent", new NginxDocItem { Syntax = "$body_bytes_sent", Description = "Number of bytes sent to a client, not counting the response header." } },
        { "$bytes_sent", new NginxDocItem { Syntax = "$bytes_sent", Description = "Number of bytes sent to a client." } },
        { "$connection", new NginxDocItem { Syntax = "$connection", Description = "Connection serial number." } },
        { "$connection_requests", new NginxDocItem { Syntax = "$connection_requests", Description = "Current number of requests made through a connection." } },
        { "$content_length", new NginxDocItem { Syntax = "$content_length", Description = "\"Content-Length\" request header field." } },
        { "$content_type", new NginxDocItem { Syntax = "$content_type", Description = "\"Content-Type\" request header field." } },
        { "$cookie_", new NginxDocItem { Syntax = "$cookie_name", Description = "The name cookie." } },
        { "$date_gmt", new NginxDocItem { Syntax = "$date_gmt", Description = "Current time in GMT. The format is suitable for use in the \"Date\" header." } },
        { "$date_local", new NginxDocItem { Syntax = "$date_local", Description = "Current time in the local time zone. The format is suitable for use in the \"Date\" header." } },
        { "$document_root", new NginxDocItem { Syntax = "$document_root", Description = "Root directory or alias for the current request." } },
        { "$document_uri", new NginxDocItem { Syntax = "$document_uri", Description = "Same as $uri." } },
        { "$fastcgi_path_info", new NginxDocItem { Syntax = "$fastcgi_path_info", Description = "The value of the second capture in the fastcgi_split_path_info directive." } },
        { "$fastcgi_script_name", new NginxDocItem { Syntax = "$fastcgi_script_name", Description = "Request URI or, if a URI ends with a slash, request URI with an index file name appended to it." } },
        { "$host", new NginxDocItem { Syntax = "$host", Description = "In this order of precedence: host name from the request line, or host name from the \"Host\" request header field, or the server name matching a request." } },
        { "$hostname", new NginxDocItem { Syntax = "$hostname", Description = "Host name." } },
        { "$http_", new NginxDocItem { Syntax = "$http_name", Description = "Arbitrary request header field; the last part of a variable name is the field name converted to lower case with dashes replaced by underscores." } },
        { "$http_user_agent", new NginxDocItem { Syntax = "$http_user_agent", Description = "The \"User-Agent\" header." } },
        { "$http_cookie", new NginxDocItem { Syntax = "$http_cookie", Description = "The \"Cookie\" header." } },
        { "$http_host", new NginxDocItem { Syntax = "$http_host", Description = "The \"Host\" header." } },
        { "$http_referer", new NginxDocItem { Syntax = "$http_referer", Description = "The \"Referer\" header." } },
        { "$http_x_forwarded_for", new NginxDocItem { Syntax = "$http_x_forwarded_for", Description = "The \"X-Forwarded-For\" header." } },
        { "$https", new NginxDocItem { Syntax = "$https", Description = "\"on\" if connection operates in SSL mode, or an empty string otherwise." } },
        { "$is_args", new NginxDocItem { Syntax = "$is_args", Description = "\"?\" if a request line has arguments, or an empty string otherwise." } },
        { "$limit_rate", new NginxDocItem { Syntax = "$limit_rate", Description = "Setting this variable enables response rate limiting." } },
        { "$msec", new NginxDocItem { Syntax = "$msec", Description = "Current time in seconds with the milliseconds resolution." } },
        { "$nginx_version", new NginxDocItem { Syntax = "$nginx_version", Description = "nginx version." } },
        { "$pid", new NginxDocItem { Syntax = "$pid", Description = "PID of the worker process." } },
        { "$pipe", new NginxDocItem { Syntax = "$pipe", Description = "\"p\" if request was pipelined, \".\" otherwise." } },
        { "$proxy_add_x_forwarded_for", new NginxDocItem { Syntax = "$proxy_add_x_forwarded_for", Description = "The \"X-Forwarded-For\" client request header field with the $remote_addr variable appended to it, separated by a comma." } },
        { "$proxy_host", new NginxDocItem { Syntax = "$proxy_host", Description = "Name and port of a proxied server as specified in the proxy_pass directive." } },
        { "$proxy_port", new NginxDocItem { Syntax = "$proxy_port", Description = "Port of a proxied server as specified in the proxy_pass directive." } },
        { "$proxy_protocol_addr", new NginxDocItem { Syntax = "$proxy_protocol_addr", Description = "Client address from the PROXY protocol header, or an empty string otherwise." } },
        { "$query_string", new NginxDocItem { Syntax = "$query_string", Description = "Same as $args." } },
        { "$realpath_root", new NginxDocItem { Syntax = "$realpath_root", Description = "Absolute path corresponding to the root directory or alias for the current request, with all symbolic links resolved to real paths." } },
        { "$remote_addr", new NginxDocItem { Syntax = "$remote_addr", Description = "Client address." } },
        { "$remote_port", new NginxDocItem { Syntax = "$remote_port", Description = "Client port." } },
        { "$remote_user", new NginxDocItem { Syntax = "$remote_user", Description = "User name supplied with the Basic authentication." } },
        { "$request", new NginxDocItem { Syntax = "$request", Description = "Full original request line." } },
        { "$request_body", new NginxDocItem { Syntax = "$request_body", Description = "Request body. The variable's value is made available in locations processed by the proxy_pass, fastcgi_pass, uwsgi_pass, and scgi_pass directives." } },
        { "$request_body_file", new NginxDocItem { Syntax = "$request_body_file", Description = "Name of a temporary file with the request body." } },
        { "$request_completion", new NginxDocItem { Syntax = "$request_completion", Description = "\"OK\" if a request has completed, or an empty string otherwise." } },
        { "$request_filename", new NginxDocItem { Syntax = "$request_filename", Description = "File path for the current request, based on the root or alias directives, and the request URI." } },
        { "$request_id", new NginxDocItem { Syntax = "$request_id", Description = "Unique request identifier generated from 16 random bytes, in hexadecimal." } },
        { "$request_length", new NginxDocItem { Syntax = "$request_length", Description = "Request length (including request line, header, and request body)." } },
        { "$request_method", new NginxDocItem { Syntax = "$request_method", Description = "Request method, usually \"GET\" or \"POST\"." } },
        { "$request_time", new NginxDocItem { Syntax = "$request_time", Description = "Request processing time in seconds with a milliseconds resolution." } },
        { "$request_uri", new NginxDocItem { Syntax = "$request_uri", Description = "Full original request URI (with arguments)." } },
        { "$scheme", new NginxDocItem { Syntax = "$scheme", Description = "Request scheme, \"http\" or \"https\"." } },
        { "$sent_http_", new NginxDocItem { Syntax = "$sent_http_name", Description = "Arbitrary response header field; the last part of a variable name is the field name converted to lower case with dashes replaced by underscores." } },
        { "$server_addr", new NginxDocItem { Syntax = "$server_addr", Description = "An address of the server which accepted a request." } },
        { "$server_name", new NginxDocItem { Syntax = "$server_name", Description = "The name of the server which accepted a request." } },
        { "$server_port", new NginxDocItem { Syntax = "$server_port", Description = "Port of the server which accepted a request." } },
        { "$server_protocol", new NginxDocItem { Syntax = "$server_protocol", Description = "Request protocol, usually \"HTTP/1.0\", \"HTTP/1.1\", or \"HTTP/2.0\"." } },
        { "$status", new NginxDocItem { Syntax = "$status", Description = "Response status." } },
        { "$tcpinfo_rtt", new NginxDocItem { Syntax = "$tcpinfo_rtt", Description = "Information about the client TCP connection; available on systems that support the TCP_INFO socket option." } },
        { "$tcpinfo_rttvar", new NginxDocItem { Syntax = "$tcpinfo_rttvar", Description = "Information about the client TCP connection; available on systems that support the TCP_INFO socket option." } },
        { "$tcpinfo_snd_cwnd", new NginxDocItem { Syntax = "$tcpinfo_snd_cwnd", Description = "Information about the client TCP connection; available on systems that support the TCP_INFO socket option." } },
        { "$tcpinfo_rcv_space", new NginxDocItem { Syntax = "$tcpinfo_rcv_space", Description = "Information about the client TCP connection; available on systems that support the TCP_INFO socket option." } },
        { "$time_iso8601", new NginxDocItem { Syntax = "$time_iso8601", Description = "Local time in the ISO 8601 standard format." } },
        { "$time_local", new NginxDocItem { Syntax = "$time_local", Description = "Local time in the Common Log Format." } },
        { "$uri", new NginxDocItem { Syntax = "$uri", Description = "Current URI in request, normalized." } },
        { "$upstream_addr", new NginxDocItem { Syntax = "$upstream_addr", Description = "Keeps the IP address and port, or the path to the UNIX-domain socket of the upstream server." } },
        { "$upstream_response_time", new NginxDocItem { Syntax = "$upstream_response_time", Description = "Keeps time spent on receiving the response from the upstream server; the time is kept in seconds with millisecond resolution." } },
        { "$upstream_status", new NginxDocItem { Syntax = "$upstream_status", Description = "Keeps status code of the response obtained from the upstream server." } },
    };
}