/**
 * AppLayout 组件的 JS 交互函数
 */
export function registerFullscreenHandler(dotNetRef) {
    window.__fullscreenDotNetRef = dotNetRef;
    if (!window.__fullscreenChangeRegistered) {
        window.__fullscreenChangeRegistered = true;
        document.addEventListener('fullscreenchange', () => {
            if (window.__fullscreenDotNetRef) {
                window.__fullscreenDotNetRef.invokeMethodAsync('OnBrowserFullscreenChanged', !!document.fullscreenElement);
            }
        });
    }
}

export function enterFullscreen() {
    document.documentElement.requestFullscreen();
}

export function exitFullscreen() {
    document.exitFullscreen();
}

