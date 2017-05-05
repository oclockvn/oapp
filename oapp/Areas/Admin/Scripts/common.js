(function () {
    /**
     * open sub menu
     */
    $(".menu").on("click", "> li > a", function (e) {
        e.preventDefault();

        var $self = $(this);
        // var $li = $self.closest("li");
        $self.toggleClass("active");

        var $sub = $self.closest("li").find(".menu--sub");
        if ($sub.length) {
            $sub.slideToggle(300);
        }
    });

    /**
     * toggle sidebar menu
     */
    $(".toggle-menu-button").on("click", function (e) {
        e.preventDefault();

        $("#header").toggleClass("mini");
        $("#sidebar").toggleClass("mini");
    });

})();