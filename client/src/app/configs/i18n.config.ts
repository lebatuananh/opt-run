export const supportedLanguages = {
    en_US: 'English',
    vi_VN: 'Việt Nam'
}

export const defaultLanguge = localStorage.getItem('lang') ||  Object.keys(supportedLanguages)[1]
