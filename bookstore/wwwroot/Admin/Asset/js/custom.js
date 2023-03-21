


$(function () {
    $("#datepicker").datepicker({
        changeMonth: true,
        changeYear: true
    });
});


    function preview_image() {
        document.querySelector('.userAvatar').src = URL.createObjectURL(document.getElementById("photoInput").files[0])
    }
