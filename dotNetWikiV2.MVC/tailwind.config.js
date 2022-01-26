module.exports = {
    content: [
        './Views/**/*.cshtml',
        './wwwroot/**/*.html'
    ],
    darkMode: "media", // or 'media' or 'class'
    theme: {
        extend: {},
    },
    variants: {
        extend: {},
    },
    plugins: [
        "autoprefixer",
        "postcss",
        require('@tailwindcss/typography')
    ],
}
