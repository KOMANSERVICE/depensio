/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["./**/*.{razor,html,cshtml}"],
    theme: {
        extend: {
            colors: {
                primary: '#A16207',
                secondary: '#FFF8F4'
            }
        },
    },
    plugins: [],
}