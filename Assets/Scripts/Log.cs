using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Log 
{
    private enum LogLevel {
    I, E, W, D
    }

    public static void debug(string msg, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "", [System.Runtime.CompilerServices.CallerFilePathAttribute] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int lineNum = -1) {
        printMessage(msg, LogLevel.D, file, callerMemberName, lineNum);        
    }

    public static void error(string msg, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "", [System.Runtime.CompilerServices.CallerFilePathAttribute] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int lineNum = -1) {
        printMessage(msg, LogLevel.E, file, callerMemberName, lineNum);        
    }
        
    public static void info(string msg, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "", [System.Runtime.CompilerServices.CallerFilePathAttribute] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int lineNum = -1) {
        printMessage(msg, LogLevel.I, file, callerMemberName, lineNum);        
    }

    private static void printMessage(string msg, LogLevel logLevel, string callerFile,  string callerMemberName, int lineNumber) {
        Debug.Log($"{logLevel}/{ProcessFile(callerFile)}.{callerMemberName}#L{lineNumber} -> {msg}");
    }

    private static string ProcessFile(string file) {
        var fileComponents =  file.Split('/');
        if (fileComponents.Length == 0) {
            return file;
        }
        var fileNameComponents = fileComponents[fileComponents.Length - 1].Split('.');
        if (fileNameComponents.Length == 0) {
            return file;
        }        
        return fileNameComponents[0];
    }
}
