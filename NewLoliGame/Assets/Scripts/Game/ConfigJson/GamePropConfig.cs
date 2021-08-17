
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Response 
/// </summary>
[Serializable]
public class GamePropConfig
{
    /// <summary>
    /// ����ʾ
    /// </summary>
    public static readonly int NO_SHOW_TYPE = -1;
    /// <summary>
    /// ����ʹ��
    /// </summary>
    public static readonly int CAN_USE_TYPE = 1;
    /// <summary>
    /// ��Ƭ����
    /// </summary>
    public static readonly int DEBRIS_PROPS_TYPE = 2;
    /// <summary>
    /// �øжȵ���
    /// </summary>
    public static readonly int FAVOR_PROPS_TYPE = 3;

    /// <summary>
    /// �������
    /// </summary>
    public static readonly int PACKAGE_PROPS_TYPE = 6;
    /// <summary>
    /// ��ɫ��������
    /// </summary>
    public static readonly int BACKGROUND_PROPS_TYPE = 7;

    public long prop_id;
	
	public string prop_name;
	
	public string description;
    // -1:����ʾ 1:����ʹ�õĵ���2����Ƭ����3:�øж�5�����޷�װ���ݶ���6:������� 7��ɫ�������ݶ���
    public int prop_type;
    // ���߼۸�0Ϊ������������0�Ϳ�����
    public double sell_price;
    // ��1��ʼ��Խ��Խ��Ʒ��
    public int quality;
    // ʹ�����ƣ�0��û��
    public int limit;
    /// <summary>
	/// ������������������, prop_id,prop_type,quantity,gravity;
	/// </summary>
    public string pack_list;

    //1:�̳�
    public string from;

    public string used;

    public TinyItem Used
    {
        get
        {
            return ItemUtil.GetTinyItem(used);
        }
    }
}
