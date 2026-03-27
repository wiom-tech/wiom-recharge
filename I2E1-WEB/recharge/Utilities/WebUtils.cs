using i2e1_basics.Utilities;
using i2e1_core.Models;
using i2e1_core.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace recharge.Utilities;

public class WebUtils
{
    private static Dictionary<string, string> fileCache = new Dictionary<string, string>();
    private static string getFileContent(string path)
    {
        if(!fileCache.TryGetValue(path, out var data))
        {
            using(StreamReader sr = new StreamReader(AppContext.BaseDirectory + path))
            {
                data = sr.ReadToEnd().Replace("\r", "");
            }
            fileCache.Add(path, data);
        }
        return data;
    }

    public static async Task<string> RenderTemplateToMqttCommand(Controller controller, string deviceType, string viewFile, RemoteManagement remoteManagement)
    {
        string path = "~/Remote";
        if (deviceType != "")
            path = path + "-" + deviceType;

        var response = await CoreWebUtils.RenderRazorViewToString(controller, path + "/" + viewFile, remoteManagement);
        string[] lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i].Trim();
            if (!string.IsNullOrEmpty(line))
            {
                sb.Append(lines[i]);
                if (i != lines.Length - 1)
                    sb.Append(" ; ");
            }
        }
        return sb.ToString();
    }

    public static void MaintainConfig(RemoteManagement remoteManagement)
    {
        switch (remoteManagement.operationType)
        {
            case OperationType.ACTIVATE_MONITOR_MODE:
                CoreDbCalls.GetInstance().MaintainConfig("monitormode;", remoteManagement.backEndNasid);
                break;
            case OperationType.ACTIVATE_CUSTOM_DNS:
                CoreDbCalls.GetInstance().MaintainConfig("customdns;", remoteManagement.backEndNasid);
                break;
            case OperationType.ACTIVATE_ONE_NETWORK:
                CoreDbCalls.GetInstance().MaintainConfig("activateonenetwork;", remoteManagement.backEndNasid);
                break;
        }
    }

    public static async Task<string> GetDeviceConfig(DeviceConfig config, Controller controller)
    {
        //this is a check to enclose the url for splash page in single qutotes 
        //as it may also contain a comma separeted splash text as well so the command line paramters have to be enclosed in quotes
        if (config.parameters.IndexOf("updatesplash") > -1)
        {
            int index1 = config.parameters.IndexOf("updatesplash");
            int index2 = config.parameters.IndexOf("splash_param_end");

            string sub = config.parameters.Substring(index1, index2 - index1);

            if (sub.IndexOf(',') > -1)
            {
                int pTo = config.parameters.LastIndexOf(" splash_param_end;");
                int pFrom = config.parameters.IndexOf("updatesplash ") + "updatesplash ".Length;

                String result = config.parameters.Substring(pFrom, pTo - pFrom);
                string newresult = "\"" + result + "\"";
                config.parameters = config.parameters.Replace(result, newresult);
            }

        }


        string commands = await CoreWebUtils.RenderRazorViewToString(controller, "~/Remote/maintainDeviceConfig.cshtml", config);
        if (config.nasid != null)
        {
            string replacestring = "nothing " + config.nasid + " " + 1 + ";";
            commands = commands.Replace("nothing;", replacestring);
            commands = commands + "rebootdevice\n";
        }
        commands = encryptcmd(commands);
        return commands;
    }

    public static string encryptcmd(string cmd)
    {
        using (MD5 md5Hash = MD5.Create())
        {
            string hash = GetMd5Hash(md5Hash, cmd);

            IEnumerable<string> parts = Split(hash, 16);
            string odd = "";
            string even = "";
            IEnumerable<string> parts1 = Split(parts.ElementAt(0), 2);
            IEnumerable<string> parts2 = Split(parts.ElementAt(1), 2);
            for (int j = 0; j < parts1.Count(); j++)
            {
                if (j % 2 == 1)
                    odd = odd + parts2.ElementAt(j) + parts1.ElementAt(j);
                else
                    even = even + parts2.ElementAt(j) + parts1.ElementAt(j);
            }
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            string padhash = GetMd5Hash(md5Hash, secondsSinceEpoch.ToString());
            string padding1 = odd + Split(padhash, 16).ElementAt(0);
            string padding2 = Split(padhash, 16).ElementAt(1) + even;
            cmd = "#starting of file " + padding1 + "\n" + cmd + "#ending of file " + padding2;
        }

        return cmd;
    }

    static IEnumerable<string> Split(string str, int chunkSize)
    {
        return Enumerable.Range(0, str.Length / chunkSize)
            .Select(i => str.Substring(i * chunkSize, chunkSize));
    }

    static string GetMd5Hash(MD5 md5Hash, string input)
    {

        // Convert the input string to a byte array and compute the hash.
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

    public static string enablefeedback(LongIdInfo nasid, string response, string operation_id,string host)
    {
        if (string.IsNullOrEmpty(operation_id))
            return response;
        string updatedresponse = string.Empty;
        string trap1 = "set -e\n";
        string trap2 = $"\nwget \"http://{host}/Remote/ScriptStatus/?nasid={nasid}&Operationid={operation_id}&status=1\" -O /dev/null \n";
        updatedresponse = trap1 + response + trap2;
        return updatedresponse;
    }
}
