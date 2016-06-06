$(document).ready(function(){
    $('[data-toggle="tooltip"]').tooltip();
});

$("#publictab").click(function(){
	$("#project_nav").show();
    $("#main_content").attr("class","col-md-9");
})

$("#dicusstab").click(function(){
	$("#project_nav").show();
    $("#main_content").attr("class","col-md-9");
})

$("#plantab").click(function(){
    $("#project_nav").hide();
    $("#main_content").attr("class","col-md-12");
});

$("#budgettab").click(function(){
    $("#project_nav").hide();
    $("#main_content").attr("class","col-md-12");
});

$("#gallerytab").click(function(){
    $("#project_nav").hide();
    $("#main_content").attr("class","col-md-12");
});

