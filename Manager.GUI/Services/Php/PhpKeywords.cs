using System.Collections.Generic;

namespace Manager.GUI.Services.Php;

public static class PhpKeywords
{
    // Comprehensive list of php.ini directives
    public static readonly List<string> All = new List<string>
    {
        // PHP Core
        "allow_url_fopen", "allow_url_include", "arg_separator.input", "arg_separator.output",
        "auto_append_file", "auto_globals_jit", "auto_prepend_file", "browse_cap",
        "default_charset", "default_mimetype", "disable_classes", "disable_functions",
        "display_errors", "display_startup_errors", "doc_root", "enable_dl",
        "enable_post_data_reading", "error_append_string", "error_log", "error_prepend_string",
        "error_reporting", "expose_php", "extension", "extension_dir", "file_uploads",
        "hard_timeout", "html_errors", "ignore_repeated_errors", "ignore_repeated_source",
        "ignore_user_abort", "implicit_flush", "include_path", "input_encoding",
        "internal_encoding", "log_errors", "log_errors_max_len", "mail.add_x_header",
        "mail.force_extra_parameters", "mail.log", "max_execution_time", "max_file_uploads",
        "max_input_nesting_level", "max_input_time", "max_input_vars", "memory_limit",
        "open_basedir", "output_buffering", "output_encoding", "output_handler",
        "post_max_size", "precision", "realpath_cache_size", "realpath_cache_ttl",
        "register_argc_argv", "report_memleaks", "report_zend_debug", "request_order",
        "sendmail_from", "sendmail_path", "serialize_precision", "short_open_tag",
        "smtp_port", "sys_temp_dir", "track_errors", "unserialize_callback_func",
        "upload_max_filesize", "upload_tmp_dir", "user_agent", "user_dir",
        "user_ini.cache_ttl", "user_ini.filename", "variables_order", "xmlrpc_error_number",
        "xmlrpc_errors", "zend.assertions", "zend.detect_unicode", "zend.enable_gc",
        "zend.multibyte", "zend.script_encoding", "zend_extension", "zlib.output_compression",
        "zlib.output_compression_level", "zlib.output_handler",

        // Date
        "date.default_latitude", "date.default_longitude", "date.sunrise_zenith",
        "date.sunset_zenith", "date.timezone",

        // Pcre
        "pcre.backtrack_limit", "pcre.jit", "pcre.recursion_limit",

        // Session
        "session.auto_start", "session.cache_expire", "session.cache_limiter",
        "session.cookie_domain", "session.cookie_httponly", "session.cookie_lifetime",
        "session.cookie_path", "session.cookie_samesite", "session.cookie_secure",
        "session.gc_divisor", "session.gc_maxlifetime", "session.gc_probability",
        "session.lazy_write", "session.name", "session.referer_check",
        "session.save_handler", "session.save_path", "session.serialize_handler",
        "session.sid_bits_per_character", "session.sid_length", "session.upload_progress.cleanup",
        "session.upload_progress.enabled", "session.upload_progress.freq",
        "session.upload_progress.min_freq", "session.upload_progress.name",
        "session.upload_progress.prefix", "session.use_cookies", "session.use_only_cookies",
        "session.use_strict_mode", "session.use_trans_sid",

        // Opcache
        "opcache.blacklist_filename", "opcache.consistency_checks", "opcache.dups_fix",
        "opcache.enable", "opcache.enable_cli", "opcache.enable_file_override",
        "opcache.error_log", "opcache.file_cache", "opcache.file_cache_consistency_checks",
        "opcache.file_cache_only", "opcache.file_update_protection", "opcache.force_restart_timeout",
        "opcache.huge_code_pages", "opcache.interned_strings_buffer", "opcache.lockfile_path",
        "opcache.log_verbosity_level", "opcache.max_accelerated_files", "opcache.max_file_size",
        "opcache.max_wasted_percentage", "opcache.memory_consumption", "opcache.opt_debug_level",
        "opcache.optimization_level", "opcache.preferred_memory_model", "opcache.preload",
        "opcache.preload_user", "opcache.protect_memory", "opcache.restrict_api",
        "opcache.revalidate_freq", "opcache.revalidate_path", "opcache.save_comments",
        "opcache.use_cwd", "opcache.validate_permission", "opcache.validate_root",
        "opcache.validate_timestamps",

        // Xdebug (Common)
        "xdebug.mode", "xdebug.start_with_request", "xdebug.client_host", "xdebug.client_port",
        "xdebug.log", "xdebug.idekey", "xdebug.max_nesting_level",
        
        // FPM (though usually in pool.d, sometimes relevant)
        "cgi.fix_pathinfo"
    };
}