function ShownHide(hideID, showID) {
    $(showID).css({
        'visibility': 'visible'
    });
    $(hideID).css({
        'visibility': 'hidden'
    });
}
function Hide(hideID) {
    $(hideID).css({
        'visibility': 'hidden'
    });
}