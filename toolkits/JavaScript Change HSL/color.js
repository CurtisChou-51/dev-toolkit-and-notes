function HSL(h, s, l) {
	if (h <= 0) { h = 0; }
	if (s <= 0) { s = 0; }
	if (l <= 0) { l = 0; }

	if (h > 360) { h = 360; }
	if (s > 100) { s = 100; }
	if (l > 100) { l = 100; }

	this.h = h;
	this.s = s;
	this.l = l;
}

function RGB(r, g, b) {
	if (r <= 0) { r = 0; }
	if (g <= 0) { g = 0; }
	if (b <= 0) { b = 0; }

	if (r > 255) { r = 255; }
	if (g > 255) { g = 255; }
	if (b > 255) { b = 255; }

	this.r = r;
	this.g = g;
	this.b = b;
}

/**
 * Color conversion utility
 * Original author: Unknown
 * Purpose: Convert colors between different formats (RGB, HSL, HEX)
 */
let converter = (function () {

	function RGBtoHSL(RGB) {
		let result = new HSL(0, 0, 0);
		let r = RGB.r / 255;
		let g = RGB.g / 255;
		let b = RGB.b / 255;

		let max = Math.max(r, g, b), min = Math.min(r, g, b);
		result.l = (max + min) / 2;

		if (max == min) {
			result.h = result.s = 0; // achromatic
		} else {
			let d = max - min;
			result.s = result.l > 0.5 ? d / (2 - max - min) : d / (max + min);

			switch (max) {
				case r: result.h = (g - b) / d + (g < b ? 6 : 0); break;
				case g: result.h = (b - r) / d + 2; break;
				case b: result.h = (r - g) / d + 4; break;
			}
		}
		result.h = Math.round(result.h * 60);
		result.s = Math.round(result.s * 1000) / 10;
		result.l = Math.round(result.l * 1000) / 10;
		return result;
	}

	function HSLtoRGB(HSL) {
		let result = new RGB(0, 0, 0);
		let h = HSL.h / 360;
		let s = HSL.s / 100;
		let l = HSL.l / 100;

		if (s == 0) {
			result.r = result.g = result.b = l; // achromatic
		} else {
			let q = l < 0.5 ? l * (1 + s) : l + s - l * s;
			let p = 2 * l - q;

			result.r = hue2rgb(p, q, h + 1 / 3);
			result.g = hue2rgb(p, q, h);
			result.b = hue2rgb(p, q, h - 1 / 3);
		}
		result.r = Math.round(result.r * 255);
		result.g = Math.round(result.g * 255);
		result.b = Math.round(result.b * 255);

		return result;
	}

	function hue2rgb(p, q, t) {
		if (t < 0) t += 1;
		if (t > 1) t -= 1;
		if (t < 1 / 6) return p + (q - p) * 6 * t;
		if (t < 1 / 2) return q;
		if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
		return p;
	}

	function HexColorCodeToHSL(hexColorCode) {
		let r = parseInt(hexColorCode.substr(1, 2), 16);
		let g = parseInt(hexColorCode.substr(3, 2), 16);
		let b = parseInt(hexColorCode.substr(5, 2), 16);
		return RGBtoHSL(new RGB(r, g, b));
	}

	function RGBToHexColorCode(RGB) {
		return "#" + ((1 << 24) + (RGB.r << 16) + (RGB.g << 8) + RGB.b).toString(16).slice(1);
	}

	return { RGBtoHSL, HSLtoRGB, HexColorCodeToHSL, RGBToHexColorCode };
})();