
using System.Collections.Generic;

public class RoleConfig
{
    public static readonly Dictionary<int, string> bodyUrl = new Dictionary<int, string>() {
        {1,"cha_yeluoli"},
        {2,"cha_lankongque"},
        {3,"cha_liangcai"},
        {4,"cha_moli"},
        {5,"cha_feiling"},
        {6,"cha_baiguangying"},
        {7,"cha_heixiangling"}
    };



    public static readonly Dictionary<int, string> roleProfessons = new Dictionary<int, string>() {
        {0,"学生"},
        {1,"公务员"},
        {2,"企业员工"},
        {3,"企业管理"},
        {4,"私营业主"},
        {5,"教师"},
        {6,"军人"},
        {7,"自由职业者"},
        {8,"其他"}
    };


    public static readonly Dictionary<int, string> fourPropertiesKeyPairs = new Dictionary<int, string>()
    {
        {1,"魅力"},
        {2,"智慧"},
        {4,"魔法"},
        {3,"环保"}
    };
}
