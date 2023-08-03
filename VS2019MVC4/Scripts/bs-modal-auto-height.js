// Bootstrap 4 Modal Auto Height
$(function () {
	if ($(".modal-autoheight").length > 0) {
		$(".modal").on("show.bs.modal", resize);
		$(window).on("resize", resize);
	}
});

function resize() {
	var winHeight = $(window).height();
	$(".modal-autoheight .modal-body").css({
		width: "auto",
		height: (winHeight - 200) + "px"
	});
}