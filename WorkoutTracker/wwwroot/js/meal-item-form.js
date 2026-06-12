'use strict';

(function () {
    const qty = document.getElementById('quantity-input');
    if (!qty) return;

    const calPerUnit = parseFloat(document.getElementById('calories-per-unit')?.value || '0') || 0;
    const protPerUnit = parseFloat(document.getElementById('protein-per-unit')?.value || '0') || 0;
    const carbsPerUnit = parseFloat(document.getElementById('carbs-per-unit')?.value || '0') || 0;
    const fatPerUnit = parseFloat(document.getElementById('fat-per-unit')?.value || '0') || 0;

    const calHidden = document.getElementById('calories-hidden');
    const protHidden = document.getElementById('protein-hidden');
    const carbHidden = document.getElementById('carbs-hidden');
    const fatHidden = document.getElementById('fat-hidden');

    const calDisplay = document.getElementById('calories-display');
    const protDisplay = document.getElementById('protein-display');
    const carbDisplay = document.getElementById('carbs-display');
    const fatDisplay = document.getElementById('fat-display');

    if (!calHidden || !protHidden || !carbHidden || !fatHidden) return;

    function formatDecimal(v) {
        const rounded = Math.round(v * 100) / 100;
        return rounded.toFixed(2).replace(/\.?0+$/, '');
    }

    function setField(hidden, display, value, asInteger) {
        const text = asInteger ? String(Math.round(value)) : formatDecimal(value);
        hidden.value = text;
        if (display) display.value = text;
    }

    function recalc() {
        const q = parseFloat(qty.value) || 1;
        setField(calHidden, calDisplay, calPerUnit * q, true);
        setField(protHidden, protDisplay, protPerUnit * q, false);
        setField(carbHidden, carbDisplay, carbsPerUnit * q, false);
        setField(fatHidden, fatDisplay, fatPerUnit * q, false);
    }

    qty.addEventListener('input', recalc);
    recalc();
})();
