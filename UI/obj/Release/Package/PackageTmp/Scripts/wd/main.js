$(document).ready(function () {
    var pageH, pageW;
    var num_1, num_2;
    var time_light;
    page = {
        init: function () {
            ////阻止body滑动
            //$('body').on("touchmove", function (e) {
            //    e.preventDefault();
            //});
            $(window).resize(function () {
                page.resize();
            });

            ////请求路径，改为自身服务器地址
            //page.URLPrefix = OriginUrl;
            //page.Project = Project;
            //page.getIntentURL = page.URLPrefix + 'api/getintent/' + page.Project;  
            //page.sendMSGlURL = page.URLPrefix + 'api/sendmsg/';                 
            //page.checkMSGlURL = page.URLPrefix + 'api/checkmsg/';               
            //page.getResult = page.URLPrefix + 'api/getresult/' + page.Project;   
            //page.subForm = page.URLPrefix + 'api/subform/';                     
            //page.Gift = 0;
            //page.start = 0;

            //page.resize();
            //page.loading.init();
            //page.p1.init();
            //page.p2.init();

        },

        resize: function () {
            pageH = $(window).height();
            pageW = $(window).width();
            $(".page").width(pageW).height(pageH);
            var num = 20 * (pageW / 320);
            num_1 = 25.875 * 1.5 / num;
            num_2 = 25.875 * 2 / num;

        }
    }


    page.init();
    page.resize();
});