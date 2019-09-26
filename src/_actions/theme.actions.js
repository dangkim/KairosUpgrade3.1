import { themeConstants } from '../_constants';
import { userService } from '../_services';

export const themeActions = {
    enableBackgroundImage,
    enableFixedHeader,
    enableHeaderShadow,
    enableSidebarShadow,
    enablePageTitleIcon,
    enablePageTitleSubheading,
    enablePageTabsAlt,
    enableFixedSidebar,
    setEnableMobileMenu,
    setEnableMobileMenuSmall,
    setEnableClosedSidebar,
    enableFixedFooter,
    backgroundColor,
    headerBackgroundColor,
    colorScheme,
    backgroundImage,
    backgroundImageOpacity
};

function enableBackgroundImage(backgroundImage) {
    return dispatch => {
        dispatch(enable(backgroundImage));
    };

    function enable(backgroundImage) { return { type: themeConstants.SET_ENABLE_BACKGROUND_IMAGE, backgroundImage } }
}

function enableFixedHeader(enableFixedHeader) {
    return dispatch => {
        dispatch(enable(enableFixedHeader));
    };

    function enable(enableFixedHeader) { return { type: themeConstants.SET_ENABLE_FIXED_HEADER, enableFixedHeader } }
}

function enableHeaderShadow(enableHeaderShadow) {
    return dispatch => {
        dispatch(enable(enableHeaderShadow));
    };

    function enable(enableHeaderShadow) { return { type: themeConstants.SET_ENABLE_HEADER_SHADOW, enableHeaderShadow } }
}

function enableSidebarShadow(enableSidebarShadow) {
    return dispatch => {
        dispatch(enable(enableSidebarShadow));
    };

    function enable(enableSidebarShadow) { return { type: themeConstants.SET_ENABLE_SIDEBAR_SHADOW, enableSidebarShadow } }
}

function enablePageTitleIcon(enablePageTitleIcon) {
    return dispatch => {
        dispatch(enable(enablePageTitleIcon));
    };

    function enable(enablePageTitleIcon) { return { type: themeConstants.SET_ENABLE_PAGETITLE_ICON, enablePageTitleIcon } }
}

function enablePageTitleSubheading(enablePageTitleSubheading) {
    return dispatch => {
        dispatch(enable(enablePageTitleSubheading));
    };

    function enable(enablePageTitleSubheading) { return { type: themeConstants.SET_ENABLE_PAGETITLE_SUBHEADING, enablePageTitleSubheading } }
}

function enablePageTabsAlt(enablePageTabsAlt) {
    return dispatch => {
        dispatch(enable(enablePageTabsAlt));
    };

    function enable(enablePageTabsAlt) { return { type: themeConstants.SET_ENABLE_PAGE_TABS_ALT, enablePageTabsAlt } }
}

function enableFixedSidebar(enableFixedSidebar) {
    return dispatch => {
        dispatch(enable(enableFixedSidebar));
    };

    function enable(enableFixedSidebar) { return { type: themeConstants.SET_ENABLE_FIXED_SIDEBAR, enableFixedSidebar } }
}

function setEnableMobileMenuSmall(enableMobileMenuSmall) {
    return dispatch => {
        dispatch(enable(enableMobileMenuSmall));
    };

    function enable(enableMobileMenuSmall) { return { type: themeConstants.SET_ENABLE_MOBILE_MENU_SMALL, enableMobileMenuSmall } }
}

function setEnableMobileMenu(enableMobileMenu) {
    return dispatch => {
        dispatch(enable(enableMobileMenu));
    };

    function enable(enableMobileMenu) { return { type: themeConstants.SET_ENABLE_MOBILE_MENU, enableMobileMenu } }
}

function setEnableClosedSidebar(enableClosedSidebar) {
    return dispatch => {
        dispatch(enable(enableClosedSidebar));
    };
    
    function enable(enableClosedSidebar) { return { type: themeConstants.SET_ENABLE_CLOSED_SIDEBAR, enableClosedSidebar } }
}

function enableFixedFooter(enableFixedFooter) {
    return dispatch => {
        dispatch(enable(enableFixedFooter));
    };

    function enable(enableFixedFooter) { return { type: themeConstants.SET_ENABLE_FIXED_FOOTER, enableFixedFooter } }
}

function backgroundColor(backgroundColor) {
    return dispatch => {
        dispatch(enable(backgroundColor));
    };

    function enable(backgroundColor) { return { type: themeConstants.SET_BACKGROUND_COLOR, backgroundColor } }
}

function headerBackgroundColor(headerBackgroundColor) {
    return dispatch => {
        dispatch(enable(headerBackgroundColor));
    };

    function enable(headerBackgroundColor) { return { type: themeConstants.SET_HEADER_BACKGROUND_COLOR, headerBackgroundColor } }
}

function colorScheme(colorScheme) {
    return dispatch => {
        dispatch(enable(colorScheme));
    };

    function enable(colorScheme) { return { type: themeConstants.SET_COLOR_SCHEME, colorScheme } }
}

function backgroundImage(enableBackgroundImage) {
    return dispatch => {
        dispatch(enable(enableBackgroundImage));
    };

    function enable(enableBackgroundImage) { return { type: themeConstants.SET_BACKGROUND_IMAGE, enableBackgroundImage } }
}

function backgroundImageOpacity(backgroundImageOpacity) {
    return dispatch => {
        dispatch(enable(backgroundImageOpacity));
    };

    function enable(backgroundImageOpacity) { return { type: themeConstants.SET_BACKGROUND_IMAGE_OPACITY, backgroundImageOpacity } }
}
    