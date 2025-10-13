window.depensio = window.depensio || {};

window.depensio = {
    initialized: function () {

        // Mobile menu toggle
        const mobileMenuButton = document.getElementById('mobile-menu-button');
        const mobileMenu = document.getElementById('mobile-menu');

        if (mobileMenuButton && mobileMenu) {
            mobileMenuButton.addEventListener('click', () => {
                mobileMenu.classList.toggle('open');
            });
        }

        // Smooth scrolling for anchor links
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();

                const targetId = this.getAttribute('href');
                if (targetId === '#') return;

                const targetElement = document.querySelector(targetId);
                if (targetElement) {
                    targetElement.scrollIntoView({ behavior: 'smooth' });

                    // Close mobile menu if open
                    if (mobileMenu && !mobileMenu.classList.contains('hidden')) {
                        mobileMenu.classList.add('hidden');
                    }
                }
            });
        });


        // Form submission
        const contactForm = document.getElementById('contactForm');
        if (contactForm) {
            contactForm.addEventListener('submit', (e) => {
                e.preventDefault();

                const name = document.getElementById('name')?.value;
                const email = document.getElementById('email')?.value;
                const subject = document.getElementById('subject')?.value;
                const message = document.getElementById('message')?.value;

                contactForm.reset();
            });
        }

        const scanTab = document.getElementById('scan-tab');
        const manualTab = document.getElementById('manual-tab');
        const scanSection = document.getElementById('scan-section');
        const manualSection = document.getElementById('manual-section');
        const scannerAnimation = document.querySelector('.scanner-animation');
        const barcodeInput = document.getElementById('barcode-input');

        if (scanTab && manualTab && scanSection && manualSection && scannerAnimation && barcodeInput) {

            // Tab switching
            scanTab.addEventListener('click', function () {
                this.classList.add('primary-text', 'border-b-2', 'primary-border');
                manualTab.classList.remove('primary-text', 'border-b-2', 'primary-border');
                manualTab.classList.add('text-gray-500');
                scanSection.classList.remove('hidden');
                manualSection.classList.add('hidden');
            });

            manualTab.addEventListener('click', function () {
                this.classList.add('primary-text', 'border-b-2', 'primary-border');
                scanTab.classList.remove('primary-text', 'border-b-2', 'primary-border');
                scanTab.classList.add('text-gray-500');
                manualSection.classList.remove('hidden');
                scanSection.classList.add('hidden');
            });

            // Focus barcode input when scanner area is clicked
            scannerAnimation.addEventListener('click', function () {
                barcodeInput.focus();
            });
        }



    },
    scrollChatToBottom: function (elementId) {
        var el = document.getElementById(elementId);
        if (el) {
            el.scrollTop = el.scrollHeight;
        }
    }
    //chatbot: function () {

    //    // Chatbot
    //    const chatbotIcon = document.getElementById('chatbotIcon');
    //    const chatbotModal = document.getElementById('chatbotModal');
    //    const closeChatbot = document.getElementById('closeChatbot');

    //    if (chatbotIcon && chatbotModal) {
    //        chatbotIcon.addEventListener('click', () => {
    //            chatbotModal.classList.toggle('hidden');
    //        });
    //    }

    //    if (closeChatbot && chatbotModal) {
    //        closeChatbot.addEventListener('click', () => {
    //            chatbotModal.classList.add('hidden');
    //        });
    //    }
    //}

}