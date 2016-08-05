$(document).ready(function () {
    //var canvas = document.getElementById("canvas");
    var canvas = $('.canvas');
    for (i = 0; i < canvas.length; i++) {

        var x = 0.75;
        //var y;
        //y = canvas[i];
        ////y = y.parent();
        ////y = y.find('.textDays');
        //console.log("y= "+y);

        var context = canvas[i].getContext("2d");

        context.beginPath();
        context.lineWidth = 1;
        context.strokeStyle = "#0ab6a4";
        context.arc(30, 30, 28, 1.5 * Math.PI, x * Math.PI);
        context.stroke();

        context.beginPath();
        context.lineWidth = 1;
        context.strokeStyle = "#e9ebec";
        context.arc(30, 30, 28, x * Math.PI, 1.5 * Math.PI);
        context.stroke();
    }
});