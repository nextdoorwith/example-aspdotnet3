// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {

    // エラーメッセージリンクをクリック時のナビゲーションカスタマイズ
    $("a[href^='#']").filter(".validation-error-link").click(function () {
        var speed = 800;
        var href = $(this).attr("href");
        var $target = $(href);
        if ($target.length > 0) {
            var position = $target.offset().top;
            $("html, body").animate({ scrollTop: position }, speed, "swing");
            $target.focus();
        }
        return false;
    });

});