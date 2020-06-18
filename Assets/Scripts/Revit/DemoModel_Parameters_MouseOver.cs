using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DemoModel_Parameters_MouseOver : MonoBehaviour
{
    public Color MouseOverColor = new Color(0.1F, 0.3F, 0.5F, 1.0F);
    public int TextSize = 14;
    private string theText = "";
    private Hashtable ht_r;
    private Hashtable ht_g;
    private Hashtable ht_b;
    private Hashtable ht_a;
    private ICollection key_ht;
    bool showText = false;
    void OnMouseUp()
    {
        showText = false;
    }

    void OnMouseDown()
    {
        showText = true;
    }

    void Start()
    {
        this.gameObject.AddComponent<MeshCollider>();
        this.gameObject.AddComponent<DemoModel_Parameters>();
        this.gameObject.GetComponent<DemoModel_Parameters>().TheList(this.gameObject.name.ToString());
        ht_r = new Hashtable();
        ht_g = new Hashtable();
        ht_b = new Hashtable();
        ht_a = new Hashtable();
        for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
        {
            ht_r.Add(i, GetComponent<Renderer>().materials[i].color.r);
            key_ht = ht_r.Keys;
            ht_g.Add(i, GetComponent<Renderer>().materials[i].color.g);
            ht_b.Add(i, GetComponent<Renderer>().materials[i].color.b);
            ht_a.Add(i, GetComponent<Renderer>().materials[i].color.a);
        }
    }
    void OnMouseOver()
    {
        for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
        {
            GetComponent<Renderer>().materials[i].color = MouseOverColor;
        }
        showText = true;
    }
    void OnMouseExit()
    {
        for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
        {
            foreach (int n in key_ht)
            {
                if (i == n)
                {
                    Color originalcolor = new Color((float)ht_r[n], (float)ht_g[n], (float)ht_b[n], (float)ht_a[n]);
                    GetComponent<Renderer>().materials[i].color = originalcolor;
                }
            }
        }
        showText = false;
    }
    void OnGUI()
    {
        if (showText)
        {
            if (this.name.ToString() != null)
            {
                string mess = null;
                foreach (string p in this.GetComponent<DemoModel_Parameters>().ParamNameList)
                {
                    mess += "\n" + p;
                    theText = this.name.ToString() + "\n" + mess;
                    GUIStyle customGuiStyle = new GUIStyle();
                    customGuiStyle.fontSize = TextSize;
                    customGuiStyle.fontStyle = FontStyle.Normal;
                    customGuiStyle.normal.textColor = MouseOverColor;
                    GUI.Label(new Rect(10, 10, Screen.width - 20, 50), theText, customGuiStyle);
                }
            }
        }
    }
}
