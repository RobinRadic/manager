using System.Collections.Generic;

namespace Manager.GUI.Services.Nginx;

public static class NginxKeywords
{
    public static readonly List<string> All = new List<string>
    {
        // --- Core & Global ---
        "daemon", "env", "debug_points", "error_log", "include", "lock_file",
        "master_process", "pid", "ssl_engine", "timer_resolution", "user",
        "worker_cpu_affinity", "worker_priority", "worker_processes",
        "worker_rlimit_core", "worker_rlimit_nofile", "worker_rlimit_sigpending",
        "working_directory", "load_module",

        // --- Events Module ---
        "events", "accept_mutex", "accept_mutex_delay", "debug_connection",
        "devpoll_changes", "devpoll_events", "epoll_events", "kqueue_changes",
        "kqueue_events", "multi_accept", "rtsig_signo", "rtsig_overflow_events",
        "rtsig_overflow_test", "rtsig_overflow_threshold", "use", "worker_connections",

        // --- HTTP Core ---
        "http", "server", "location", "root", "alias", "index", "try_files",
        "error_page", "recursive_error_pages", "port_in_redirect", "server_name_in_redirect",
        "absolute_redirect", "msie_padding", "msie_refresh", "log_not_found",
        "keepalive_disable", "keepalive_requests", "keepalive_timeout",
        "types", "default_type", "types_hash_max_size", "types_hash_bucket_size",
        "client_body_buffer_size", "client_body_in_file_only", "client_body_in_single_buffer",
        "client_body_temp_path", "client_body_timeout", "client_header_buffer_size",
        "client_header_timeout", "client_max_body_size", "large_client_header_buffers",
        "ignore_invalid_headers", "underscores_in_headers", "connection_pool_size",
        "request_pool_size", "if_modified_since", "max_ranges", "output_buffers",
        "postpone_output", "read_ahead", "reset_timedout_connection", "send_timeout",
        "sendfile", "sendfile_max_chunk", "tcp_nodelay", "tcp_nopush",
        "server_name", "server_names_hash_max_size", "server_names_hash_bucket_size",
        "listen", "resolver", "resolver_timeout", "satisfy", "internal", "merge_slashes",

        // --- Logging ---
        "access_log", "log_format", "open_log_file_cache",

        // --- Gzip ---
        "gzip", "gzip_buffers", "gzip_comp_level", "gzip_disable", "gzip_http_version",
        "gzip_min_length", "gzip_proxied", "gzip_types", "gzip_vary", "gzip_static",
        "gunzip", "gunzip_buffers",

        // --- SSL / TLS ---
        "ssl", "ssl_buffer_size", "ssl_certificate", "ssl_certificate_key",
        "ssl_ciphers", "ssl_client_certificate", "ssl_crl", "ssl_dhparam",
        "ssl_ecdh_curve", "ssl_password_file", "ssl_prefer_server_ciphers",
        "ssl_protocols", "ssl_session_cache", "ssl_session_ticket_key",
        "ssl_session_tickets", "ssl_session_timeout", "ssl_stapling",
        "ssl_stapling_file", "ssl_stapling_responder", "ssl_stapling_verify",
        "ssl_trusted_certificate", "ssl_verify_client", "ssl_verify_depth",

        // --- Proxy ---
        "proxy_pass", "proxy_bind", "proxy_buffer_size", "proxy_buffering",
        "proxy_buffers", "proxy_busy_buffers_size", "proxy_cache", "proxy_cache_background_update",
        "proxy_cache_bypass", "proxy_cache_convert_head", "proxy_cache_key",
        "proxy_cache_lock", "proxy_cache_lock_age", "proxy_cache_lock_timeout",
        "proxy_cache_max_range_offset", "proxy_cache_methods", "proxy_cache_min_uses",
        "proxy_cache_path", "proxy_cache_purge", "proxy_cache_revalidate",
        "proxy_cache_use_stale", "proxy_cache_valid", "proxy_connect_timeout",
        "proxy_cookie_domain", "proxy_cookie_path", "proxy_force_ranges",
        "proxy_headers_hash_bucket_size", "proxy_headers_hash_max_size",
        "proxy_hide_header", "proxy_http_version", "proxy_ignore_client_abort",
        "proxy_ignore_headers", "proxy_intercept_errors", "proxy_limit_rate",
        "proxy_max_temp_file_size", "proxy_method", "proxy_next_upstream",
        "proxy_next_upstream_timeout", "proxy_next_upstream_tries", "proxy_no_cache",
        "proxy_pass_header", "proxy_pass_request_body", "proxy_pass_request_headers",
        "proxy_read_timeout", "proxy_redirect", "proxy_request_buffering",
        "proxy_send_lowat", "proxy_send_timeout", "proxy_set_body", "proxy_set_header",
        "proxy_ssl_certificate", "proxy_ssl_certificate_key", "proxy_ssl_ciphers",
        "proxy_ssl_crl", "proxy_ssl_name", "proxy_ssl_password_file",
        "proxy_ssl_server_name", "proxy_ssl_session_reuse", "proxy_ssl_protocols",
        "proxy_ssl_trusted_certificate", "proxy_ssl_verify", "proxy_ssl_verify_depth",
        "proxy_store", "proxy_store_access", "proxy_temp_file_write_size",
        "proxy_temp_path",

        // --- FastCGI (PHP, etc) ---
        "fastcgi_pass", "fastcgi_bind", "fastcgi_buffer_size", "fastcgi_buffering",
        "fastcgi_buffers", "fastcgi_busy_buffers_size", "fastcgi_cache",
        "fastcgi_cache_background_update", "fastcgi_cache_bypass", "fastcgi_cache_key",
        "fastcgi_cache_lock", "fastcgi_cache_lock_age", "fastcgi_cache_lock_timeout",
        "fastcgi_cache_max_range_offset", "fastcgi_cache_methods", "fastcgi_cache_min_uses",
        "fastcgi_cache_path", "fastcgi_cache_purge", "fastcgi_cache_revalidate",
        "fastcgi_cache_use_stale", "fastcgi_cache_valid", "fastcgi_catch_stderr",
        "fastcgi_connect_timeout", "fastcgi_force_ranges", "fastcgi_hide_header",
        "fastcgi_ignore_client_abort", "fastcgi_ignore_headers", "fastcgi_index",
        "fastcgi_intercept_errors", "fastcgi_keep_conn", "fastcgi_limit_rate",
        "fastcgi_max_temp_file_size", "fastcgi_next_upstream", "fastcgi_next_upstream_timeout",
        "fastcgi_next_upstream_tries", "fastcgi_no_cache", "fastcgi_param",
        "fastcgi_pass_header", "fastcgi_pass_request_body", "fastcgi_pass_request_headers",
        "fastcgi_read_timeout", "fastcgi_request_buffering", "fastcgi_send_lowat",
        "fastcgi_send_timeout", "fastcgi_split_path_info", "fastcgi_store",
        "fastcgi_store_access", "fastcgi_temp_file_write_size", "fastcgi_temp_path",

        // --- Upstream ---
        "upstream", "ip_hash", "least_conn", "least_time", "keepalive", "ntlm",
        "sticky", "zone", "hash", "random", "queue",

        // --- Access & Auth ---
        "allow", "deny", "auth_basic", "auth_basic_user_file",
        "auth_request", "auth_request_set",

        // --- Headers & Expires ---
        "add_header", "expires", "add_trailer",

        // --- Rewrites & Logic ---
        "rewrite", "return", "break", "if", "set", "uninitialized_variable_warn",
        "rewrite_log", "valid_referers",

        // --- Real IP ---
        "set_real_ip_from", "real_ip_header", "real_ip_recursive",

        // --- Limits ---
        "limit_conn", "limit_conn_log_level", "limit_conn_status", "limit_conn_zone",
        "limit_rate", "limit_rate_after", "limit_req", "limit_req_log_level",
        "limit_req_status", "limit_req_zone",

        // --- Map & Split ---
        "map", "map_hash_bucket_size", "map_hash_max_size", "split_clients",

        // --- Charset ---
        "charset", "charset_map", "charset_types", "override_charset", "source_charset",

        // --- Auto Index ---
        "autoindex", "autoindex_exact_size", "autoindex_format", "autoindex_localtime",

        // --- Browser ---
        "ancient_browser", "ancient_browser_value", "modern_browser", "modern_browser_value",

        // --- HTTP Variables ---
        "$arg_", "$args", "$binary_remote_addr", "$body_bytes_sent",
        "$bytes_sent", "$connection", "$connection_requests", "$content_length",
        "$content_type", "$cookie_", "$date_gmt", "$date_local", "$document_root",
        "$document_uri", "$fastcgi_path_info", "$fastcgi_script_name", "$host",
        "$hostname", "$http_", "$http_user_agent", "$http_cookie", "$http_host",
        "$http_referer", "$http_x_forwarded_for", "$https", "$is_args",
        "$limit_rate", "$msec", "$nginx_version", "$pid", "$pipe",
        "$proxy_add_x_forwarded_for", "$proxy_host", "$proxy_port", "$proxy_protocol_addr",
        "$query_string", "$realpath_root", "$remote_addr", "$remote_port",
        "$remote_user", "$request", "$request_body", "$request_body_file",
        "$request_completion", "$request_filename", "$request_id", "$request_length",
        "$request_method", "$request_time", "$request_uri", "$scheme",
        "$sent_http_", "$server_addr", "$server_name", "$server_port",
        "$server_protocol", "$status", "$tcpinfo_rtt", "$tcpinfo_rttvar",
        "$tcpinfo_snd_cwnd", "$tcpinfo_rcv_space", "$time_iso8601", "$time_local",
        "$uri", "$upstream_addr", "$upstream_response_time", "$upstream_status"
    };
}