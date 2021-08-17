
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Response 
/// </summary>
[Serializable]
public class GameFavourPropConfig
{ 
	
	public int prop_id;
	
	public int wang_mo;
	
	public int chen_si_si;
	
	public int jian_peng;
	
	public int shu_yan;
	
	public int qi_na;
	
	public int gao_tai_ming;
	
	public int wen_qian;
	
	public int feng_yin_sha;
	
	public int shui_wang_zi;
	
	public int xin_ling;
	
	public int huang_shi;
	
	public int yan_jue;
	
	public int pang_zun;
	
	public int jin_wang_zi;
	
	public int ling_gong_zhu;
	
	public int man_duo_la;
	
	public int huo_ling_zhu;
	
	public int du_niang_niang;
	
	public int bing_gong_zhu;
	
	public int mo_sha;

    Dictionary<int, int> _favourDics;
    
    public Dictionary<int, int> FavorDic
    {
        get
        {
            if(_favourDics == null)
            {
                _favourDics = new Dictionary<int, int>();
                _favourDics.Add(8, wang_mo);
                _favourDics.Add(9, chen_si_si);
                _favourDics.Add(10, jian_peng);
                _favourDics.Add(11, shu_yan);
                _favourDics.Add(12, qi_na);
                _favourDics.Add(13, gao_tai_ming);
                _favourDics.Add(14, wen_qian);
                _favourDics.Add(15, feng_yin_sha);
                _favourDics.Add(16, shui_wang_zi);
                _favourDics.Add(17, man_duo_la);
                _favourDics.Add(18, xin_ling);
                _favourDics.Add(19, bing_gong_zhu);
                _favourDics.Add(20, huang_shi);
                _favourDics.Add(21, mo_sha);
                _favourDics.Add(22, pang_zun);
                _favourDics.Add(23, yan_jue);
                _favourDics.Add(24, du_niang_niang);
                _favourDics.Add(25, jin_wang_zi);
                _favourDics.Add(26, ling_gong_zhu);
                _favourDics.Add(27, huo_ling_zhu);
            }
            return _favourDics;
        }
    }
}
