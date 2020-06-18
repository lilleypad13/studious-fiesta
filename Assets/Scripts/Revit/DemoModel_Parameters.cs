using UnityEngine;
using System.Collections.Generic;

public class DemoModel_Parameters : MonoBehaviour
{
    private List<string> paramNameList = new List<string>();

    public List<string> ParamNameList
    {
        get
        {
            return paramNameList;
        }
        set
        {
            paramNameList = value;
        }
    }

    public void TheList(string value)
    {
        if (value != null)
        {
            paramNameList.Add(ParamByName(value));
            paramNameList.Sort();
        }
    }

    private string ParamByName(string d)
    {
        switch (d)
    {

        case "Generic_-_12\"_[354721]":
            d = "Area : 8100 ft2 \nElevation at Bottom : -1 ft \nElevation at Top : 0 ft \nType Name : Generic - 12\" \n";
            break;
        case "Generic_-_8\"_[354755]":
            d = "Area : 780 ft2 \nLength : 89.33 ft \nType Name : Generic - 8\" \nWidth : 0.67 ft \n";
            break;
        case "Generic_-_8\"_[354778]":
            d = "Area : 893.33 ft2 \nLength : 89.33 ft \nType Name : Generic - 8\" \nWidth : 0.67 ft \n";
            break;
        case "Generic_-_8\"_[354806]":
            d = "Area : 482.21 ft2 \nLength : 89.33 ft \nType Name : Generic - 8\" \nWidth : 0.67 ft \n";
            break;
        case "Generic_-_8\"_[354829]":
            d = "Area : 886.67 ft2 \nLength : 89.33 ft \nType Name : Generic - 8\" \nWidth : 0.67 ft \n";
            break;
        case "Generic_-_5\"_[354885]":
            d = "Area : 358.33 ft2 \nLength : 35.42 ft \nType Name : Generic - 5\" \nWidth : 0.42 ft \n";
            break;
        case "Generic_-_5\"_[354913]":
            d = "Area : 183.17 ft2 \nLength : 20.42 ft \nType Name : Generic - 5\" \nWidth : 0.42 ft \n";
            break;
        case "Generic_-_5\"_[354939]":
            d = "Area : 354.17 ft2 \nLength : 35.42 ft \nType Name : Generic - 5\" \nWidth : 0.42 ft \n";
            break;
        case "Generic_-_5\"_[354959]":
            d = "Area : 200 ft2 \nLength : 20.42 ft \nType Name : Generic - 5\" \nWidth : 0.42 ft \n";
            break;
        case "Generic_-_5\"_[354985]":
            d = "Area : 587.33 ft2 \nLength : 60.42 ft \nType Name : Generic - 5\" \nWidth : 0.42 ft \n";
            break;
        case "Generic_-_5\"_[355015]":
            d = "Area : 354.17 ft2 \nLength : 35.42 ft \nType Name : Generic - 5\" \nWidth : 0.42 ft \n";
            break;
        case "Generic_-_5\"_[355049]":
            d = "Area : 604.17 ft2 \nLength : 60.42 ft \nType Name : Generic - 5\" \nWidth : 0.42 ft \n";
            break;
        case "Generic_-_5\"_[355084]":
            d = "Area : 350 ft2 \nLength : 35.42 ft \nType Name : Generic - 5\" \nWidth : 0.42 ft \n";
            break;
        case "36\"_x_84\"_[355142]":
            d = "Area : 34.37 ft2 \nHeight : 7 ft \nType Name : 36\" x 84\" \nVisual Light Transmittance : 0 \nWidth : 3 ft \n";
            break;
        case "36\"_x_84\"_[355209]":
            d = "Area : 34.37 ft2 \nHeight : 7 ft \nType Name : 36\" x 84\" \nVisual Light Transmittance : 0 \nWidth : 3 ft \n";
            break;
        case "Glazed_[355710]":
            d = "Area : 390.12 ft2 \nHeight : 6 ft \nType Name : Glazed \nVisual Light Transmittance : 0.9 \nWidth : 65.02 ft \n";
            break;
        case "Glazed_[356004]":
            d = "Area : 140 ft2 \nHeight : 7 ft \nType Name : Glazed \nVisual Light Transmittance : 0.9 \nWidth : 20 ft \n";
            break;
        case "36\"_x_84\"_[356050]":
            d = "Area : 34.37 ft2 \nHeight : 7 ft \nType Name : 36\" x 84\" \nVisual Light Transmittance : 0 \nWidth : 3 ft \n";
            break;
    }
        return d;
    }
}

