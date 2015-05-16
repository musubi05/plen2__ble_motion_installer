using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLEN.MFX
{
    public class BLEMfxCommand
    {
        public TagMfxModel xmlMfx;
        public string strConvertedMfx;
        public string strConvertedMfxForDisplay;
        public bool isMfxConverted = false;

        public BLEMfxCommand(TagMfxModel mfxModel)
        {
            xmlMfx = mfxModel;
        }
        public string Name
        {
            get
            {
                // 先頭のモーションのNameをこのクラスのNameとして返す（特に深い意味はない）
                if (xmlMfx.Motion.Count > 0)
                    return xmlMfx.Motion[0].Name;
                else
                    return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>false：失敗，true：成功</returns>
        public bool convertMfxCommand()
        {
            strConvertedMfx = "";
            strConvertedMfxForDisplay = "";
            isMfxConverted = false;
            try
            {
                foreach (TagMotionModel tagMotion in xmlMfx.Motion)
                {
                    /*----- システムコマンド「slotNum」 -----*/
                    strConvertedMfx += int.Parse(tagMotion.ID).ToString("x2");
                    strConvertedMfxForDisplay += "[slotNum : " + int.Parse(tagMotion.ID).ToString("x2") + "] ";

                    /*----- システムコマンド「name」 -----*/
                    strConvertedMfx += string.Format("{0,-20}", tagMotion.Name);
                    strConvertedMfxForDisplay += "[name : " + string.Format("{0,-20}", tagMotion.Name) + "] ";  
     
                    /*----- システムコマンド「config」 -----*/
                    // Paramはid:0，id:1の2つしかない
                    if (tagMotion.Extra.Param.Count != 2)
                        return false;
                    // ParamリストをIDの昇順に並び替え
                    tagMotion.Extra.Param.Sort((o1, o2) => o1.ID.CompareTo(o2.ID));
                    // コマンドを追加
                    strConvertedMfx += byte.Parse(tagMotion.Extra.Function).ToString("x2");
                    strConvertedMfx += byte.Parse(tagMotion.Extra.Param[0].Param).ToString("x2");
                    strConvertedMfx += byte.Parse(tagMotion.Extra.Param[1].Param).ToString("x2");
                    strConvertedMfxForDisplay += "[config : " + int.Parse(tagMotion.Extra.Function).ToString("x2");
                    strConvertedMfxForDisplay += byte.Parse(tagMotion.Extra.Param[0].Param).ToString("x2");
                    strConvertedMfxForDisplay += byte.Parse(tagMotion.Extra.Param[1].Param).ToString("x2") + "] ";

                    /*----- システムコマンド「frameNum」 -----*/
                    strConvertedMfx += int.Parse(tagMotion.FrameNum).ToString("x2");
                    strConvertedMfxForDisplay += "[frameNum : " + byte.Parse(tagMotion.FrameNum).ToString("x2") + "] ";

                    /*----- システムコマンド「frame」 -----*/
                    // FrameリストをIDの昇順に並び替え
                    tagMotion.Frame.Sort((o1, o2) => o1.ID.CompareTo(o2.ID));
                    strConvertedMfxForDisplay += "[frame : ";
                    foreach (TagFrameModel tagFrame in tagMotion.Frame)
                    {
                        strConvertedMfx += short.Parse(tagFrame.Time).ToString("x4");
                        strConvertedMfxForDisplay += " " + short.Parse(tagFrame.Time).ToString("x4");
                        // JointリストをIDの昇順に並べ替え
                        tagFrame.Joint.Sort((o1, o2) => o1.ID.CompareTo(o2.ID));
                        foreach (TagJointModel tagJoint in tagFrame.Joint)
                        {
                            strConvertedMfx += short.Parse(tagJoint.Joint).ToString("x4");
                            strConvertedMfxForDisplay += short.Parse(tagJoint.Joint).ToString("x4");
                        }
                    }
                    strConvertedMfxForDisplay += "]";
                }
            }
            catch (Exception)
            {
                return false;
            }

            isMfxConverted = true;
            int i = strConvertedMfx.Length;
            return true;
        }
    }




    [System.Xml.Serialization.XmlRoot("mfx")]
    public class TagMfxModel
    {
        [System.Xml.Serialization.XmlElement("motion")]
        public List<PLEN.MFX.TagMotionModel> Motion { get; set; }
    }

    public class TagMotionModel
    {
        [System.Xml.Serialization.XmlAttribute("id")]
        public String ID { get; set; }

        [System.Xml.Serialization.XmlElement("name")]
        public string Name { get; set; }

        [System.Xml.Serialization.XmlElement("extra")]
        public TagExtraModel Extra { get; set; }

        [System.Xml.Serialization.XmlElement("frameNum")]
        public string FrameNum { get; set; }

        [System.Xml.Serialization.XmlElement("frame")]
        public List<TagFrameModel> Frame { get; set; }
    }

    public class TagExtraModel
    {

        [System.Xml.Serialization.XmlElement("function")]
        public string Function { get; set; }

        [System.Xml.Serialization.XmlElement("param")]
        public List<TagParamModel> Param { get; set; }
    }

    public class TagParamModel
    {
        [System.Xml.Serialization.XmlAttribute("id")]
        public String ID { get; set; }

        [System.Xml.Serialization.XmlText()]
        public string Param { get; set; }
    }


    public class TagFrameModel
    {
        [System.Xml.Serialization.XmlAttribute("id")]
        public String ID { get; set; }

        [System.Xml.Serialization.XmlElement("time")]
        public string Time { get; set; }

        [System.Xml.Serialization.XmlElement("joint")]
        public List<TagJointModel> Joint { get; set; }
    }

    public class TagJointModel
    {
        [System.Xml.Serialization.XmlAttribute("id")]
        public String ID { get; set; }

        [System.Xml.Serialization.XmlText()]
        public string Joint { get; set; }
    }
}
