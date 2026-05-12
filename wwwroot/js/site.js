
(function () {
    var loader = document.getElementById('page-loader');

    function showLoader() {
        if (loader) loader.classList.remove('loader-hidden');
    }

    function hideLoader() {
        if (loader) loader.classList.add('loader-hidden');
    }

    window.addEventListener('load', hideLoader);

    window.addEventListener('beforeunload', showLoader);

    document.addEventListener('DOMContentLoaded', function () {
        if (window.jQuery) {
            $(document).ajaxStart(showLoader).ajaxStop(hideLoader);
        }
    });
})();
