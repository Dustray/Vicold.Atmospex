using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace Vicold.Atmospex.Configration;
public static class ConfigReader
{
    /// <summary>
    /// 加载Json配置文件
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="configPath"></param>
    /// <returns></returns>
    public static async Task<TEntity> LoadJsonConfig<TEntity>(string configPath) where TEntity : class
    {
        return await LoadJsonToAsync<TEntity>(configPath);
    }

    /// <summary>
    /// 加载XML配置文件
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="configPath"></param>
    /// <returns></returns>
    public static async Task<TEntity> LoadXMLConfig<TEntity>(string configPath) where TEntity : class
    {
        return await LoadXMLToAsync<TEntity>(configPath);
    }

    private static Task<TEntity> LoadXMLToAsync<TEntity>(string xmlFilePath) where TEntity : class
    {
        var tcsResult = new TaskCompletionSource<TEntity>();
        try
        {
            if (!File.Exists(xmlFilePath))
            {
                var ex = new Exception("对不起！未能找到需要加载的XML文件；请确认文件是否存在，及路径是否正确。");
                tcsResult.SetException(ex);
            }
            else
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);
                var root = xmlDoc.DocumentElement;
                var tmpString = JsonConvert.SerializeXmlNode(root, Newtonsoft.Json.Formatting.None, true);
                var result = JsonConvert.DeserializeObject<TEntity>(tmpString);
                tcsResult.SetResult(result);
            }
        }
        catch (Exception ex)
        {
            tcsResult.SetException(ex);
        }
        return tcsResult.Task;
    }

    private static Task<TEntity> LoadJsonToAsync<TEntity>(string jsonFilePath) where TEntity : class
    {
        var tcsResult = new TaskCompletionSource<TEntity>();
        try
        {
            if (!File.Exists(jsonFilePath))
            {
                var ex = new Exception("对不起！未能找到需要加载的Json文件；请确认文件是否存在，及路径是否正确。");
                tcsResult.SetException(ex);
            }
            else
            {
                var json = File.ReadAllText(jsonFilePath);
                var result = JsonConvert.DeserializeObject<TEntity>(json);
                tcsResult.SetResult(result);
            }
        }
        catch (Exception ex)
        {
            tcsResult.SetException(ex);
        }

        return tcsResult.Task;
    }
}
