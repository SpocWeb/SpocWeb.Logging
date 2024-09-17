﻿using System.Text;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Serilog.Parsing;

namespace org.SpocWeb.root.Logging;

/// <summary> Extension Methods to use <see cref="StringInterpolationWithValues"/> for Logging </summary>
/// <remarks>This makes Log-Statements more readable and re-usable for Exceptions and other Messages</remarks>
public static class Log
{
#pragma warning disable CA2254
    static readonly MessageTemplateParser _messageTemplateParser = new();

    // Function to generate a formatted string
    // Parse the Serilog message template
    public static MessageTemplate ParseTemplate(string message, object parameters) => _messageTemplateParser.Parse(message);

    static readonly Dictionary<string, MessageTemplate> _templates = new();

    /// <summary> Parses and caches the <paramref name="stringInterpolation"/> </summary>
    public static StringInterpolationWithValues Parse(Exception? x, string stringInterpolation, params object[] args)
    {
        if (!_templates.TryGetValue(stringInterpolation, out var template))
        {
            _templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
        }

        return new StringInterpolationWithValues(template, args) { Exception = x };
    }

    #region Log Statements

    public static StringInterpolationWithValues Error(this ILogger log, FormattableString stringInterpolation, Exception? x = null)
        => Error(log, Parse(stringInterpolation), x);

    public static StringInterpolationWithValues Error(this ILogger log, StringInterpolationWithValues parsed, Exception? x = null)
    {
        log.LogError(x, parsed.template.Text, parsed.values);
        return parsed;
    }

    public static StringInterpolationWithValues Critical(this ILogger log, FormattableString stringInterpolation, Exception? x = null)
        => Critical(log, Parse(stringInterpolation), x);

    public static StringInterpolationWithValues Critical(this ILogger log, StringInterpolationWithValues parsed, Exception? x = null)
    {
        log.LogCritical(x, parsed.template.Text, parsed.values);
        return parsed;
    }

    public static StringInterpolationWithValues Debug(this ILogger log, FormattableString stringInterpolation, Exception? x = null)
        => Debug(log, Parse(stringInterpolation), x);

    public static StringInterpolationWithValues Debug(this ILogger log, StringInterpolationWithValues parsed, Exception? x = null)
    {
        log.LogDebug(x, parsed.template.Text, parsed.values);
        return parsed;
    }

    public static StringInterpolationWithValues Information(this ILogger log, FormattableString stringInterpolation, Exception? x = null)
        => Information(log, Parse(stringInterpolation), x);

    public static StringInterpolationWithValues Information(this ILogger log, StringInterpolationWithValues parsed, Exception? x = null)
    {
        log.LogInformation(x, parsed.template.Text, parsed.values);
        return parsed;
    }

    public static StringInterpolationWithValues Warning(this ILogger log, FormattableString stringInterpolation, Exception? x = null)
        => Warning(log, Parse(stringInterpolation), x);

    public static StringInterpolationWithValues Warning(this ILogger log, StringInterpolationWithValues parsed, Exception? x = null)
    {
        log.LogWarning(x, parsed.template.Text, parsed.values);
        return parsed;
    }

    public static StringInterpolationWithValues Trace(this ILogger log, FormattableString stringInterpolation, Exception? x = null)
        => Trace(log, Parse(stringInterpolation), x);

    public static StringInterpolationWithValues Trace(this ILogger log, StringInterpolationWithValues parsed, Exception? x = null)
    {
        log.LogTrace(x, parsed.template.Text, parsed.values);
        return parsed;
    }

    #endregion Log Statements

    public static string Format(this MessageTemplate template, params object?[] properties)
    {
        var result = new StringBuilder(template.Text);
        int pos = -1;
        foreach (var token in template.Tokens)
        {
            if (token is not PropertyToken propertyToken)
            {
                continue;
            }

            var propertyValue = int.TryParse(propertyToken.PropertyName, out var index)
                ? properties[index]?.ToString()
                : properties[++pos]?.ToString();
            result = result.Replace($"{{{propertyToken.PropertyName}}}", propertyValue);
        }

        return result.ToString();
    }

    /// <inheritdoc cref="AddProperties"/>
    public static Dictionary<string, object?> ToDictionary(this MessageTemplate template, object?[] properties)
    {
        var dictionary = new Dictionary<string, object?>();
        dictionary.AddProperties(template, properties);
        return dictionary;
    }

    /// <summary> Adds the <paramref name="properties"/> to the <paramref name="dictionary"/> </summary>
    public static IDictionary<string, object?> AddProperties(this IDictionary<string, object?>? dictionary, MessageTemplate template, params object?[] properties)
    {
        dictionary ??= new Dictionary<string, object?>();
        int pos = -1;
        foreach (var token in template.Tokens)
        {
            if (token is not PropertyToken propertyToken)
            {
                continue;
            }

            var propertyValue = int.TryParse(propertyToken.PropertyName, out var index)
                ? properties[index]?.ToString()
                : properties[++pos]?.ToString();
            dictionary[propertyToken.PropertyName] = propertyValue;
        }

        return dictionary;
    }

    public static string Format(this MessageTemplate template, IReadOnlyDictionary<string, object?> properties)
    {
        var result = new StringBuilder(template.Text);
        var tokens = template.Tokens.OfType<PropertyToken>();
        foreach (var token in tokens)
        {
            var propertyValue = properties[token.PropertyName]?.ToString();
            result = result.Replace($"{{{token.PropertyName}}}", propertyValue);
        }

        return result.ToString();
    }


    /// <summary> Parses and caches the <paramref name="stringInterpolation"/> </summary>
    public static StringInterpolationWithValues Parse(this FormattableString stringInterpolation) {
	    if (!_templates.TryGetValue(stringInterpolation.Format, out var template)) {
		    _templates[stringInterpolation.Format] = template = _messageTemplateParser.Parse(stringInterpolation.Format);
	    }

	    return new StringInterpolationWithValues(template, stringInterpolation.GetArguments());
    }

    #region parsing with > 0 Params 

    /// <summary> Parses and caches the <paramref name="stringInterpolation"/> </summary>
    public static StringInterpolationWithValues Parse(string stringInterpolation, object? arg0)
    {
        if (!_templates.TryGetValue(stringInterpolation, out var template))
        {
            _templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
        }

        return new StringInterpolationWithValues(template, arg0);
    }

    public static StringInterpolationWithValues Parse(string formatString, object? arg0, object? arg1)
    {
        if (!_templates.TryGetValue(formatString, out var template))
        {
            _templates[formatString] = template = _messageTemplateParser.Parse(formatString);
        }

        return new StringInterpolationWithValues(template, arg0, arg1);
    }

    public static StringInterpolationWithValues Parse(string stringInterpolation
        , object? arg0, object? arg1, object? arg2)
    {
        if (!_templates.TryGetValue(stringInterpolation, out var template))
        {
            _templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
        }

        return new StringInterpolationWithValues(template, arg0, arg1, arg2);
    }

    public static StringInterpolationWithValues Parse(string stringInterpolation, object? arg0, object? arg1, object? arg2, object? arg3)
    {
        if (!_templates.TryGetValue(stringInterpolation, out var template))
        {
            _templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
        }

        return new StringInterpolationWithValues(template, arg0, arg1, arg2, arg3);
    }

    public static StringInterpolationWithValues Parse(string stringInterpolation
        , object? arg0, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        if (!_templates.TryGetValue(stringInterpolation, out var template))
        {
            _templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
        }

        return new StringInterpolationWithValues(template, arg0, arg1, arg2, arg3, arg4);
    }

    public static StringInterpolationWithValues Parse(string stringInterpolation
        , object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5)
    {
        if (!_templates.TryGetValue(stringInterpolation, out var template))
        {
            _templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
        }

        return new StringInterpolationWithValues(template, arg0, arg1, arg2, arg3, arg4, arg5);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Method has 10 parameters, which is greater than the 7 authorized.", Justification = "Cannot use params[]")]
    public static StringInterpolationWithValues Parse(string stringInterpolation
        , object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6)
    {
        if (!_templates.TryGetValue(stringInterpolation, out var template))
        {
            _templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
        }

        return new StringInterpolationWithValues(template, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Method has 10 parameters, which is greater than the 7 authorized.", Justification = "Cannot use params[]")]
    public static StringInterpolationWithValues Parse(string stringInterpolation
        , object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7)
    {
        if (!_templates.TryGetValue(stringInterpolation, out var template))
        {
            _templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
        }

        return new StringInterpolationWithValues(template, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Method has 10 parameters, which is greater than the 7 authorized.", Justification = "Cannot use params[]")]
    public static StringInterpolationWithValues Parse(string stringInterpolation
        , object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7, object? arg8)
    {
        if (!_templates.TryGetValue(stringInterpolation, out var template))
        {
            _templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
        }

        return new StringInterpolationWithValues(template, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Method has 10 parameters, which is greater than the 7 authorized.", Justification = "Cannot use params[]")]
    public static StringInterpolationWithValues Parse(string stringInterpolation, object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7, object? arg8, object? arg9)
    {
        if (!_templates.TryGetValue(stringInterpolation, out var template))
        {
            _templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
        }

        return new StringInterpolationWithValues(template, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }

    #endregion parsing with > 0 Params 

}
