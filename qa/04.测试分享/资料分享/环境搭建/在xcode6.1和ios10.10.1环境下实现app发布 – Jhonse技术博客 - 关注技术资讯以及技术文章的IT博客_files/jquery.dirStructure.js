(function($){
    $.fn.dirStructure = function(options){
        var defaults = {
            
        };
        var options = $.extend(defaults, options);
        this.each(function(){
            $(this).append("<div class='dirStructure_line'></div>");
            $(this).append("<div class='dirStructure_h4_line'></div>");
            $(this).append("<div class='dirStructure_data'></div>");
            $(this).append("<div class='dirStructure_end'>End</div>");
            var dirStructure_this = this;
            var first_h3_top = 117;
            //获得h3元素
            $(this).find("h3").each(function(index_h3){
                $(this).append("<div class='dirStructure_h3'>"+(index_h3+1)+"</div>");
                $(dirStructure_this).find(".dirStructure_data").append("<div class='dirStructure_data_index dirStructure_data_index_"+(index_h3+1)+"'>"+(index_h3+1)+"</div>");
                //获得介于h3之间的元素
                var index_h4 = 1;
                $(dirStructure_this).find(".dirStructure_data").find(".dirStructure_data_index_" + (index_h3+1)).append("<div class='dirStructure_h4_nav'></div>");
                $(this).nextUntil("h3").each(function(){
                    if($(this).is("h4")){
                        //判断是否为h4元素
                        $(this).css({"padding-left":"10px"});
                        $(this).append("<div class='dirStructure_h4'>"+index_h4+"</div>");
                        $(dirStructure_this).find(".dirStructure_data").find(".dirStructure_data_index_" + (index_h3+1)).find(".dirStructure_h4_nav").append("<div class='dirStructure_h4_nav_son'>"+index_h4+"</div>");
                        index_h4++;
                    }else if($(this).is("p")){
                        //判断是否为p元素
                        $(this).css({"padding-left":"28px"});
                    }
                });
            });
            
            $(window).scroll(function(){
                var window_scrollTop = $(this).scrollTop();
                var dirStructure_this_top = $(dirStructure_this).offset().top;
                var dirStructure_end_top = $(dirStructure_this).find(".dirStructure_end").offset().top;
                var change = true;
                if(window_scrollTop < dirStructure_this_top){
                    first_h3_top = 0;
                }else{
                     first_h3_top = 117;
                }
                $(dirStructure_this).find("h3").each(function(index){
                    var dirStructure_data_height = $(dirStructure_this).find(".dirStructure_data").height();
                    var h3_top = $(this).offset().top;
                    if(window_scrollTop + dirStructure_data_height + first_h3_top >= h3_top){
                        $(this).find(".dirStructure_h3").hide();
                        $(dirStructure_this).find(".dirStructure_data").find(".dirStructure_data_index").eq(index).show();
                        $(dirStructure_this).find(".dirStructure_data").find(".dirStructure_data_index").eq((parseInt((index+1)/10)-1)*10+(parseInt((index+1)%10)-1)).hide();
                    }else{
                        $(this).find(".dirStructure_h3").show();
                        $(dirStructure_this).find(".dirStructure_data").find(".dirStructure_data_index").eq(index).hide();
                    }
                    if(dirStructure_end_top - window_scrollTop - 97 <= dirStructure_data_height){
                        change = false;
                    }
                    var index_h4 = 0;
                    var index_h4_hide = 0;
                    $(this).nextUntil("h3").each(function(){
                        if($(this).is("h4")){
                            var dirStructure_h4_nav_height = $(dirStructure_this).find(".dirStructure_data").find(".dirStructure_data_index").eq(index).find(".dirStructure_h4_nav").height();
                            var h4_top = $(this).offset().top;
                            var first_h4_top = first_h3_top + (32+5)*index;
                            if(window_scrollTop + dirStructure_h4_nav_height + first_h4_top >= h4_top){
                                $(this).find(".dirStructure_h4").hide();
                                $(dirStructure_this).find(".dirStructure_data").find(".dirStructure_data_index").eq(index).find(".dirStructure_h4_nav").find(".dirStructure_h4_nav_son").eq(index_h4).show();
                                index_h4_hide ++;
                            }else{
                                $(this).find(".dirStructure_h4").show();
                                $(dirStructure_this).find(".dirStructure_data").find(".dirStructure_data_index").eq(index).find(".dirStructure_h4_nav").find(".dirStructure_h4_nav_son").eq(index_h4).hide();
                            }
                            index_h4++;
                        }
                    });
                    if(index_h4 === index_h4_hide){
                        $(dirStructure_this).find(".dirStructure_data").find(".dirStructure_data_index").eq(index).find(".dirStructure_h4_nav").find(".dirStructure_h4_nav_son").hide();
                    }
                });
                if(change){
                    $(dirStructure_this).find(".dirStructure_data").removeAttr("style"); //ie,ff均支持
                    $(dirStructure_this).find(".dirStructure_data").attr("style","");   //ff支持，ie不支持 
                    $(dirStructure_this).find(".dirStructure_data").css({"position":"fixed","left":$(dirStructure_this).offset().left,"top":first_h3_top});
                }else{
                    $(dirStructure_this).find(".dirStructure_data").removeAttr("style"); //ie,ff均支持
                    $(dirStructure_this).find(".dirStructure_data").attr("style","");   //ff支持，ie不支持 
                    $(dirStructure_this).find(".dirStructure_data").css({"position":"absolute","left":0,"bottom":"20px","height":"auto"});
                }
            });
            
            $(this).find(".dirStructure_data").find(".dirStructure_data_index").each(function(index){
                $(this).click(function(){
                    var dirStructure_data_height = $(dirStructure_this).find(".dirStructure_data").height();
                    var pos = $(dirStructure_this).find("h3").eq(index).offset().top - dirStructure_data_height - first_h3_top;
                    $("html,body").animate({scrollTop: pos}, 500);
                });
            });
        });
    };
})(jQuery);


