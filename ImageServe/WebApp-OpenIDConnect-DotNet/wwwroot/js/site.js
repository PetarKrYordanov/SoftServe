// Write your Javascript code.

$(document).ready(function () {
    $("#like-btn").click(function () {
        let user = $(this).attr("user");
        let image = $(this).attr("image");
        let url = "/image/togglelike?userId=" + user + "&imageId=" + image;
        $.getJSON(url)
            .done(function (data) {
                $("#like-btn")
                    .removeClass(data.liked ? "unchecked" : "checked")
                    .addClass(data.liked ? "checked" : "unchecked");
                $("#likes").html("Likes: " + data.likes);
                $("#likes-error").html("");
            })
            .fail(function () {
                $("#likes-error").html("An error occured please try again later.");
            });
    });
});


$(document).ready(function () {
    $.getJSON(
        '/profile/all',
        function (users) {
            $('#privacy').autocomplete({
                source: users
            });
        });
});

$(document).ready(function () {

    $(".zoom").hover(function () {
        $(this).addClass('transition');
    }, function () {
        $(this).removeClass('transition');
    });
});
