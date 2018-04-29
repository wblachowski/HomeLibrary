$(document).ready(function () {
    $(".dropdown-trigger").dropdown();
    $('.modal-trigger').leanModal();
});

function onInvitationSuccess(result) {
    console.log(result);
}