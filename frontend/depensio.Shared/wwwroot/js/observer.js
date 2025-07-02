//// wwwroot/js/observer.js
//window.observeScroll = (id, dotnetHelper) => {
//    const el = document.getElementById(id);
//    if (!el) return;

//    const observer = new IntersectionObserver(entries => {
//        entries.forEach(entry => {
//            if (entry.isIntersecting) {
//                dotnetHelper.invokeMethodAsync("OnInView", id);
//            }
//        });
//    });

//    observer.observe(el);
//};

//window.observeFadeIn = function (id) {
//    const el = document.getElementById(id);
//    if (!el) return;

//    el.classList.add('opacity-0', 'translate-y-4', 'transition-all', 'duration-700');

//    const observer = new IntersectionObserver(entries => {
//        entries.forEach(entry => {
//            if (entry.isIntersecting) {
//                el.classList.remove('opacity-0', 'translate-y-4');
//                el.classList.add('opacity-100', 'translate-y-0');
//            }
//        });
//    });

//    observer.observe(el);
//};