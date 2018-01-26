using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFixtureProject.Helpers
{
    public class TestFixtureCommands
    {
        //KD Debug
        public static string _mredledcommd = "cmd=setlightColor" + "&" + "id=0" + "&" + "idType=d" + "&" + "zone=000000" + "&" + "color=FF000000";
        public static string _mblueledcomm = "cmd=setlightColor" + "&" + "id=0" + "&" + "idType=d" + "&" + "zone=000000" + "&" + "color=0000FF00";
        public static string _mgreenledcomm = "cmd=setlightColor" + "&" + "id=0" + "&" + "idType=d" + "&" + "zone=000000" + "&" + "color=00FF0000";
        public static string _mwhiteledcomm = "cmd=setlightColor" + "&" + "id=0" + "&" + "idType=d" + "&" + "zone=000000" + "&" + "color=000000FF";
        public static string _mblendedwhiteledcomm = "cmd=setlightColor" + "&" + "id=0" + "&" + "idType=d" + "&" + "zone=000000" + "&" + "color=3AFF23FF";
        public static string _mmagendaledcomm = "cmd=setlightColor" + "&" + "id=0" + "&" + "idType=d" + "&" + "zone=000000" + "&" + "color=FF003300";
        //led commands
        //public static string _mredledcommd = "cmd=setlightColor" + "&" + "id=0" + "&" + "idType=d" + "&" + "zone=000000" + "&" + "color=3F000000";
        ////public static string _mredledcommd = "cmd=setlightColor" + "&" + "id=0" + "&" + "idType=d" + "&" + "zone=000000" + "&" + "color=FF000000";
        //public static string _mblueledcomm = "cmd=setlightColor" + "&" + "id=0" + "&" + "idType=d" + "&" + "zone=000000" + "&" + "color=003CFF00";
        //public static string _mgreenledcomm = "cmd=setlightColor" + "&" + "id=0" + "&" + "idType=d" + "&" + "zone=000000" + "&" + "color=00FF0000";
        //public static string _mwhiteledcomm = "cmd=setlightColor" + "&" + "id=0" + "&" + "idType=d" + "&" + "zone=000000" + "&" + "color=CCFF4CFF";
        //product type command
        public static string _mgetProductType = "cmd=getProductType" + "&" + "nodeId=" + "&" + "uid=" + "&" + "filename=" + "&" + "utctime=" + "&" + "ip=" + "&" + "mask=" + "&" + "gateway=" + "&" + "dns=" + "&" + "ship=" + "&" + "shfld=" + "&" + "hosts=" + "&" + "shared=";
        public static string _mgetVersionCommd = "cmd=getVersion" + "&" + "nodeId=" + "&" + "uid=" + "&" + "filename=" + "&" + "utctime=" + "&" + "ip=" + "&" + "mask=" + "&" + "gateway=" + "&" + "dns=" + "&" + "ship=" + "&" + "shfld=" + "&" + "hosts=" + "&" + "shared=";
        //Mirror commands
        public static string _mtoprightcommand = "cmd=set" + "&" + "id=0" + "&" + "idType=d" + "&" + "featureName=mirror" + "&" + "featureCmd=set" + "&" + "value=";
        public static string _mtoplefttcommand = "cmd=set" + "&" + "id=0" + "&" + "idType=d" + "&" + "featureName=mirror" + "&" + "featureCmd=set" + "&" + "value=";
        public static string _mbottomrightcommand = "cmd=set" + "&" + "id=0" + "&" + "idType=d" + "&" + "featureName=mirror" + "&" + "featureCmd=set" + "&" + "value=";
        public static string _mbottomlefttcommand = "cmd=set" + "&" + "id=0" + "&" + "idType=d" + "&" + "featureName=mirror" + "&" + "featureCmd=set" + "&" + "value=";

        //mirror home command
        public static string _mhomecommand = "cmd=set" + "&" + "id=0" + "&" + "idType=d" + "&" + "featureName=mirror" + "&" + "featureCmd=set" + "&" + "value=5000,5000,0";

        //version command
        //Switch off light command

        public static string _mswitchofflightcommand = "cmd=setlightColor" + "&" + "id=" + "&" + "idType=d" + "&" + "zone=" + "&" + "color=000000" + "&" + "featureName=zone" + "&" + "featureCmd =zone" + "&" + "value=0000000";

        //start the image show for mirror callibration
        public static string _mstartImgShow = "cmd=startImage" + "&" + "id=" + "&" + "idType=d" + "&" + "filename=White--Dot.png";

        ////start the image show for mirror callibration
        //public static string _mstartImgShow = "cmd=startImage" + "&" + "id=" + "&" + "idType=d" + "&" + "filename=White--Dot.png";

        //stop image for mirror callibration
        public static string _mstopimageshow = "cmd=stopImage" + "&" + "id=" + "&" + "idType=d" + "&" + "filename=White--Dot.png";

        //start the image show to project
        public static string _mstartImgShow1 = "cmd=startImage" + "&" + "id=" + "&" + "idType=d" + "&" + "filename=test-focus.png";

        //stop image for mirror to project
        public static string _mstopimageshow1 = "cmd=stopImage" + "&" + "id=" + "&" + "idType=d" + "&" + "filename=test-focus.png";

        //mirro command to project image on screen

        public static string _mhomescreenmirrorcommand = "cmd=set" + "&" + "id=0" + "&" + "idType=d" + "&" + "featureName=mirror" + "&" + "featureCmd=set" + "&" + "value=";
    }
}
