!function(t){function e(n){if(r[n])return r[n].exports;var o=r[n]={i:n,l:!1,exports:{}};return t[n].call(o.exports,o,o.exports,e),o.l=!0,o.exports}var r={};e.m=t,e.c=r,e.d=function(t,r,n){e.o(t,r)||Object.defineProperty(t,r,{configurable:!1,enumerable:!0,get:n})},e.n=function(t){var r=t&&t.__esModule?function(){return t.default}:function(){return t};return e.d(r,"a",r),r},e.o=function(t,e){return Object.prototype.hasOwnProperty.call(t,e)},e.p="",e(e.s=5)}({0:function(t,e,r){"use strict";function n(t,e){for(var r=0;r<e.length;r++){var n=e[r];n.enumerable=n.enumerable||!1,n.configurable=!0,"value"in n&&(n.writable=!0),Object.defineProperty(t,n.key,n)}}Object.defineProperty(e,"__esModule",{value:!0}),e.GistHelper=e.dataGistIdKey=void 0;var o="data-gist-id";e.dataGistIdKey=o;var i=function(){function t(){!function(t,e){if(!(t instanceof e))throw new TypeError("Cannot call a class as a function")}(this,t)}return function(t,e,r){e&&n(t.prototype,e),r&&n(t,r)}(t,[{key:"findAndLoadGists",value:function(){var t=document.querySelectorAll("[".concat(o,"]")),e=!0,r=!1,n=void 0;try{for(var i,a=t[Symbol.iterator]();!(e=(i=a.next()).done);e=!0){var l=i.value;this.loadGistForElement(l)}}catch(t){r=!0,n=t}finally{try{e||null==a.return||a.return()}finally{if(r)throw n}}}},{key:"loadGistForElement",value:function(t){var e=t.getAttribute(o),r=document.createElement("iframe");r.src="/gist-loader.html?".concat(o,"=").concat(e),r.frameBorder="0",r.scrolling="no",r.className="s-gist",r.onload=this.gistLoadedCallback,t.parentNode.replaceChild(r,t),console.log("Loading gist:",e)}},{key:"gistLoadedCallback",value:function(t){this.style.height=this.contentWindow.document.body.scrollHeight+"px"}}]),t}();e.GistHelper=i},5:function(t,e,r){"use strict";function n(t,e){if(Array.isArray(t))return t;if(Symbol.iterator in Object(t))return function(t,e){var r=[],n=!0,o=!1,i=void 0;try{for(var a,l=t[Symbol.iterator]();!(n=(a=l.next()).done)&&(r.push(a.value),!e||r.length!==e);n=!0);}catch(t){o=!0,i=t}finally{try{n||null==l.return||l.return()}finally{if(o)throw i}}return r}(t,e);throw new TypeError("Invalid attempt to destructure non-iterable instance")}var o=r(0),i=function(t){return t?(/^[?#]/.test(t)?t.slice(1):t).split("&").reduce(function(t,e){var r=n(e.split("="),2),o=r[0],i=r[1];return t[o]=i?decodeURIComponent(i.replace(/\+/g," ")):"",t},{}):{}}(document.location.search)[o.dataGistIdKey];if(null==i)throw new Error("Error, gist id is null.");var a='<script src="https://gist.github.com/'.concat(i,'.js"><\/script>');document.write(a),console.log("Gist loader active for gist:",i)}});
//# sourceMappingURL=gist-loader.js.map