using System.Text;

namespace Daily.WPF.Helper;
/// <summary>
/// 一个用于密码加密的类
/// </summary>
public static class PasswordMd5
{
    /// <summary>
    /// 将密码加密
    /// </summary>
    /// <param name="password">密码</param>
    /// <returns>加密后的密码</returns>
    public static string Md5(string password)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();//实例化一个md5对像
        var inputBytes = Encoding.UTF8.GetBytes(password);//将传入的字符串转为字节数组
        var hashBytes = md5.ComputeHash(inputBytes);//调用md5的ComputeHash方法将字节数组加密
        var sb = new StringBuilder();//创建一个新的Stringbuilder
        foreach (var t in hashBytes)//遍历加密后的字节数组
        {
            sb.Append(t.ToString("X2"));//将字节转为十六进制表示的字符串
        }
        return sb.ToString();
    }
}
