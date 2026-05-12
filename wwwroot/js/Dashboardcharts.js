

"use strict";

const intelligenceColors = {
    "الذكاء اللغوي": "#2a6967",
    "الذكاء المنطقي": "#32c8c3",
    "الذكاء البصري": "#c948ff",
    "الذكاء المكاني": "#c948ff",
    "الذكاء الجسدي": "#9d4ebe",
    "الذكاء الحركي": "#9d4ebe",
    "الذكاء الموسيقي": "#ffb90f",
    "الذكاء الاجتماعي": "#ff8282",
    "الذكاء الذاتي": "#ff6531",
    "الذكاء الطبيعي": "#ff3c4b"
};

const _colorPalette = Object.values(intelligenceColors);

function getIntelligenceColor(name, fallback) {
    if (!name) return fallback || _colorPalette[0];
    if (intelligenceColors[name]) return intelligenceColors[name];
    for (const [key, color] of Object.entries(intelligenceColors)) {
        const keyword = key.replace("الذكاء ", "");
        if (name.includes(keyword)) return color;
    }
    const idx = [...name].reduce((s, c) => s + c.charCodeAt(0), 0) % _colorPalette.length;
    return _colorPalette[idx];
}

function normalizeRating(r) {
    return (r || "").trim().replace("ً", "ا");
}

function ratingTextColor(r) {
    const n = normalizeRating(r);
    if (n.includes("غالب")) return "#1a7a77"; 
    if (n.includes("أحيان") || n.includes("احيان")) return "#7a5800"; 
    return "#cc3300";                                                  
}

function ratingBgColor(r) {
    const n = normalizeRating(r);
    if (n.includes("غالب")) return "#d4f5f3";
    if (n.includes("أحيان") || n.includes("احيان")) return "#fff3d4";
    return "#ffe0d4";
}

function ratingBadgeStyle(r) {
    return `background:${ratingBgColor(r)};color:${ratingTextColor(r)};
            border:1px solid ${ratingTextColor(r)}40;border-radius:4px;
            padding:1px 6px;font-size:11px;font-weight:600`;
}


function initDashboard(barLabels, barData, effortData, trendData, detailData) {
    initBarAndRadarCharts(barLabels, barData, detailData);
    initEffortChart(effortData);
    initTrendChart(trendData);
    initIntelDetail(detailData);
}


function initBarAndRadarCharts(barLabels, barData) {
    const barCanvas = document.getElementById("barChart");
    if (barCanvas) {
        if (!barLabels.length) {
            showEmptyState(barCanvas, "لا تتوفر بيانات ذكاء لعرض هذا المخطط.");
        } else {
            const barColors = barLabels.map(l => intelligenceColors[l] || "#CCCCCC");
            new Chart(barCanvas, {
                type: "bar",
                data: {
                    labels: barLabels,
                    datasets: [{ label: "", data: barData, backgroundColor: barColors }]
                },
                options: {
                    indexAxis: "y",
                    plugins: { legend: { display: false } },
                    scales: { x: { min: 0, max: 100 } }
                }
            });
        }
        initGaugeRow(detailData);

    }
    function initGaugeRow(detailData) {
        const row = document.getElementById("gaugeRow");
        if (!row || !detailData || !detailData.length) return;

        detailData.forEach(d => {
            const color = intelligenceColors[d.name] || "#888780";
            const card = document.createElement("div");
            card.style.cssText = `
            background:var(--color-background-primary);
            border:0.5px solid var(--color-border-tertiary);
            border-radius:var(--border-radius-lg);
            padding:14px 10px 10px;
            min-width:145px;max-width:165px;
            flex-shrink:0;
            display:flex;flex-direction:column;
            align-items:center;gap:4px;`;

            card.innerHTML = `
            <div style="font-size:12px;font-weight:500;
                        color:var(--color-text-secondary);
                        text-align:center;line-height:1.3;
                        margin-bottom:2px">${d.name}</div>
            ${buildGaugeSVG(d.score, color)}
            <div style="font-size:20px;font-weight:500;
                        color:var(--color-text-primary)">${Math.round(d.score)}%</div>
            ${d.isUnderMeasurement
                    ? `<div style="font-size:11px;color:#c0392b;
                               background:#fff0ed;
                               border:1px solid #e85d4a;
                               border-radius:6px;padding:3px 8px;
                               text-align:center;line-height:1.4;
                               margin-top:2px">قيد القياس</div>`
                    : ""}`;
            row.appendChild(card);
        });
    }

    function buildGaugeSVG(score, color) {
        const r = 52, cx = 70, cy = 68;
        const pct = Math.min(Math.max(score, 0), 100) / 100;
        const startAngle = Math.PI;
        const endAngle = 2 * Math.PI;
        const angle = startAngle + pct * (endAngle - startAngle);

        function arcPath(a1, a2) {
            const x1 = cx + r * Math.cos(a1), y1 = cy + r * Math.sin(a1);
            const x2 = cx + r * Math.cos(a2), y2 = cy + r * Math.sin(a2);
            const large = (a2 - a1) > Math.PI ? 1 : 0;
            return `M${x1},${y1} A${r},${r} 0 ${large},1 ${x2},${y2}`;
        }

        const dot = pct > 0
            ? `<circle cx="${cx + r * Math.cos(angle)}"
                   cy="${cy + r * Math.sin(angle)}"
                   r="5" fill="${color}"/>`
            : "";

        return `<svg width="140" height="80" viewBox="0 0 140 80" aria-hidden="true">
        <path d="${arcPath(startAngle, endAngle)}"
              fill="none" stroke="#d3d1c7" stroke-width="10" stroke-linecap="round"/>
        ${pct > 0
                ? `<path d="${arcPath(startAngle, angle)}"
                     fill="none" stroke="${color}" stroke-width="10"
                     stroke-linecap="round"/>`
                : ""}
        ${dot}
    </svg>`;
    }
}

function initEffortChart(effortData) {
    const canvas = document.getElementById("effortChart");
    if (!canvas) return;

    if (!effortData || !effortData.length) {
        showEmptyState(canvas, "لا تتوفر بيانات كافية لعرض مخطط الجهد مقابل الدرجة بعد.");
        return;
    }

    const colors = Object.values(intelligenceColors);
    const datasets = effortData.map((d, i) => ({
        label: d.intelligenceName,
        data: [{ x: d.totalTimeSec || 0, y: d.score || 0, r: 6 + (d.sessionCount || 1) * 5 }],
        backgroundColor: (colors[i] || "#ccc") + "BB",
        borderColor: colors[i] || "#ccc",
        borderWidth: 1.5
    }));

    new Chart(canvas, {
        type: "bubble",
        data: { datasets },
        options: {
            responsive: true,
            layout: { padding: 16 },
            plugins: {
                legend: { display: true, position: "bottom" }  
            },
            scales: {
                x: { title: { display: true, text: "الوقت الكلي (ثانية)" }, min: 0 },
                y: { title: { display: true, text: "الدرجة" }, min: 0, max: 100 }
            }
        }
    });
}
function initTrendChart(trendData) {
    const canvas = document.getElementById("trendChart");
    if (!canvas) return;

    const dates = [...new Set(trendData.map(p => p.date))].sort();

    if (dates.length ==0) {
        showEmptyState(
            canvas,
            dates.length === 0
                ? "لا توجد جلسات مسجلة بعد لعرض منحنى الأداء."
                : "يحتاج المنحنى إلى أكثر من جلسة واحدة لعرض التطور عبر الزمن."
        );
        return;
    }

    const intels = [...new Set(trendData.map(p => p.intel))];
    const datasets = intels.map(name => {
        const color = intelligenceColors[name] || "#ccc";
        return {
            label: name,
            data: dates.map(d => {
                const pt = trendData.find(p => p.date === d && p.intel === name);
                return pt ? +(pt.score * 100).toFixed(1) : null;
            }),
            borderColor: color,
            backgroundColor: "transparent",
            pointBackgroundColor: color,
            borderWidth: 1.5,
            pointRadius: 6,
            spanGaps: true,
            tension: 0.3
        };
    });

    new Chart(canvas, {
        type: "line",
        data: { labels: dates, datasets },
        options: {
            responsive: true,
            plugins: { legend: { display: true, position: "bottom" } },
            scales: { y: { min: 0, max: 100, ticks: { callback: v => v + "%" } } }
        }
    });
}


function initIntelDetail(detailData) {
    if (!detailData.length) return;

    renderIntelDetail(detailData, detailData[0].id);

    document.getElementById("intelSelect")?.addEventListener("change", function () {
        renderIntelDetail(detailData, this.value);
    });
}


function renderIntelDetail(detailData, id) {
    const d = detailData.find(x => String(x.id) === String(id));
    if (!d) return;

    const panel = document.getElementById("intelDetailPanel");
    const heatmapTableId  = "heatTable_"      + Date.now();
    const chartContainerId = "groupedBarWrap_" + Date.now();
    const groupedBarId    = "groupedBar_"      + Date.now();
    const assessPanelId   = "assessPanel_"     + Date.now();
    let groupedChart = null;

    //Levels summary card 
    let levelsHtml = "";
    if (!d.levels || d.levels.length === 0) {
        levelsHtml = `<p class="text-muted" style="font-size:13px">لا توجد مستويات محددة لهذا الذكاء.</p>`;
    } else {
        const listItems = d.levels.map(l => {
            const played = l.isPlayed;
            return `
                <li class="d-flex align-items-center gap-2 py-2 border-bottom" style="list-style:none">
                    <span style="display:inline-block;width:10px;height:10px;border-radius:50%;
                                 background:${played ? "#32c8c3" : "#ced4da"};flex-shrink:0"></span>
                    <span style="font-size:13px;color:${played ? "#1a7a77" : "#6c757d"};
                                 font-weight:${played ? "600" : "400"}">${l.name}</span>
                    ${played
                        ? `<span class="ms-auto badge" style="background:#d4f5f3;color:#1a7a77;font-size:11px">مكتمل ✓</span>`
                        : `<span class="ms-auto badge" style="background:#f8f9fa;color:#adb5bd;font-size:11px">لم يُلعب</span>`}
                </li>`;
        }).join("");

        levelsHtml = `
            <div class="card border-0 shadow-sm mb-4">
                <div class="card-body">
                    <p class="text-muted mb-1" style="font-size:12px;text-transform:uppercase;letter-spacing:.05em">المستويات</p>
                    <div class="mb-3 d-flex align-items-center gap-2">
                        <span class="fw-bold" style="font-size:22px">${d.completedLevels ?? 0}</span>
                        <span class="text-muted" style="font-size:15px">/ ${d.totalLevels}</span>
                        <span class="text-muted" style="font-size:13px">مستويات</span>
                    </div>
                    <ul class="ps-0 mb-0">${listItems}</ul>
                </div>
            </div>`;
    }

    //Clickable aspect  above the heatmap 
    let levelPillsHtml = "";
    const hasLevels  = d.levels  && d.levels.length  > 0;
    const hasAspects = d.aspects && d.aspects.length > 0;
    const pillColors = ["#8ee4e0", "#32c8c3", "#c9a3db", "#ffb90f", "#ff8282"];
    if (hasAspects) {
        const pills = d.aspects.map((a, i) => {
            const c = pillColors[i] || "#888";
            return `
            <button type="button" class="heat-level-pill" data-aspect-idx="${i}"
                    style="background:${c}22;color:${c};border:1.5px solid ${c};
                           border-radius:20px;padding:5px 14px;font-size:12px;
                           font-weight:600;cursor:pointer;
                           display:inline-flex;align-items:center;gap:6px;
                           white-space:nowrap;transition:all 0.15s ease;">
                ${a.name}
            </button>`;
        }).join("");

        levelPillsHtml = `
            <div style="display:flex;flex-wrap:wrap;gap:8px;margin-bottom:14px;align-items:center">
                <span style="font-size:12px;color:var(--color-text-secondary);font-weight:500;flex-shrink:0">الجوانب:</span>
                ${pills}
            </div>`;
    }

    //Heatmap table 
    let heatHtml = "";
    if (!hasAspects) {
        heatHtml = `<p class="text-muted" style="font-size:13px">لا تتوفر بيانات جوانب لهذا الذكاء.</p>`;
    } else {
        const allIndicators = [...new Set(
            d.aspects.flatMap(a => a.indicators.map(i => i.name))
        )];

        if (!allIndicators.length) {
            heatHtml = `<p class="text-muted" style="font-size:13px">لا تتوفر بيانات مؤشرات لهذا الذكاء.</p>`;
        } else {
            let rows = "";
            allIndicators.forEach(indName => {
                const rowAspects = d.aspects
                    .map((a, i) => a.indicators.some(ind => ind.name === indName) ? i : -1)
                    .filter(i => i >= 0).join(',');
                rows += `<tr data-aspects="${rowAspects}"><td class="text-end fw-semibold" style="font-size:12px;min-width:160px;padding:6px 10px">${indName}</td>`;
                d.aspects.forEach((a, colIdx) => {
                    const ind = a.indicators.find(i => i.name === indName);
                    if (ind) {
                        const pct = Math.round(ind.score * 100);
                        rows += `
                            <td data-col-idx="${colIdx}"
                                style="background:${ratingBgColor(ind.rating)};
                                       color:${ratingTextColor(ind.rating)};
                                       font-weight:600;text-align:center;
                                       padding:6px 8px;font-size:12px;
                                       transition:opacity 0.2s ease">
                                ${pct}%<br>
                                <small style="font-size:10px;font-weight:400">${ind.rating}</small>
                            </td>`;
                    } else {
                        rows += `<td data-col-idx="${colIdx}" style="text-align:center;color:#adb5bd;font-size:13px;transition:opacity 0.2s ease">—</td>`;
                    }
                });
                rows += `</tr>`;
            });

            heatHtml = `
                <div class="table-responsive">
                    <table class="dash-table table-sm align-middle mb-0" id="${heatmapTableId}" style="font-size:12px">
                        <thead class="dash-thead-yellow">
                            <tr>
                                <th class="text-end" style="min-width:160px">المؤشر</th>
                                ${d.aspects.map((a, i) => `<th data-col-idx="${i}" style="text-align:center;transition:opacity 0.2s ease">${a.name}</th>`).join("")}
                            </tr>
                        </thead>
                        <tbody>${rows}</tbody>
                    </table>
                </div>`;
        }
    }

    //Grouped bar chart
    let groupedHtml = "";
    if (!hasAspects) {
        groupedHtml = `<p class="text-muted" style="font-size:13px">لا تتوفر بيانات لعرض المخطط المجمع.</p>`;
    } else {
        groupedHtml = `
            <div id="${chartContainerId}" style="position:relative;height:280px">
                <canvas id="${groupedBarId}"
                        aria-label="مخطط شريطي مجمع للجوانب والمؤشرات"
                        style="cursor:pointer"></canvas>
            </div>
            <p class="text-muted mt-2 mb-0" style="font-size:11px;text-align:center">
                انقر على شريط مؤشر لعرض تفاصيل التقييم
            </p>
            <div id="${assessPanelId}" class="mt-3"></div>`;
    }

    const levelsMetaBox = d.totalLevels > 0
        ? `<div class="intel-meta-box"><span>المستويات المنجزة</span><h5>${d.completedLevels ?? 0} / ${d.totalLevels}</h5></div>`
        : "";

    const underMeasurementBanner = d.isUnderMeasurement ? `
        <div style="background-color:#fff0ed;border:1.5px solid #e85d4a;border-radius:10px;
                    padding:12px 18px;margin-bottom:18px;color:#c0392b;font-weight:600;
                    font-size:0.92rem;display:flex;align-items:center;gap:10px;">
            <i class="bi bi-exclamation-circle-fill" style="font-size:1.2rem;flex-shrink:0"></i>
            هذا الذكاء لا يزال قيد القياس — لم يُكمل الطفل جميع مستويات هذه المرحلة بعد.
        </div>` : "";

    panel.innerHTML = `
        <div class="d-flex flex-wrap gap-3 mb-4 justify-content-center">
            <div class="intel-meta-box"><span>الذكاء</span><h5>${d.name}</h5></div>
            <div class="intel-meta-box"><span>الدرجة</span><h5>${d.score}</h5></div>
            ${levelsMetaBox}
        </div>

        ${underMeasurementBanner}

        ${levelsHtml}

        <div class="dash-chart-box">
            <span class="dash-section-title">خريطة المؤشرات الحرارية</span>
            ${levelPillsHtml}
            ${heatHtml}
        </div>

        <div class="dash-chart-box">
            <span class="dash-section-title">الجوانب والمؤشرات — مخطط مجمع</span>
            ${groupedHtml}
        </div>`;

    //heatmap filter + chart rebuild per aspect 
    function applyHeatmapFilter(aspectIdx) {
        const table = document.getElementById(heatmapTableId);
        if (!table) return;
        table.querySelectorAll('tbody tr').forEach(row => {
            const rowAspects = (row.dataset.aspects || '').split(',').map(Number);
            row.style.display = rowAspects.includes(aspectIdx) ? '' : 'none';
        });
        table.querySelectorAll('[data-col-idx]').forEach(el => {
            el.style.display = parseInt(el.dataset.colIdx) === aspectIdx ? '' : 'none';
        });
    }

    function renderAspectChart(aspectIdx) {
        if (groupedChart) { groupedChart.destroy(); groupedChart = null; }
        const canvas = document.getElementById(groupedBarId);
        if (!canvas) return;
        const aspect = d.aspects[aspectIdx];
        if (!aspect) return;

        const color = pillColors[aspectIdx] || '#ccc';
        const labels = aspect.indicators.map(i => i.name);

        groupedChart = new Chart(canvas, {
            type: 'bar',
            data: {
                labels,
                datasets: [{
                    label: aspect.name,
                    data: aspect.indicators.map(i => +(i.score * 100).toFixed(1)),
                    backgroundColor: color + 'CC',
                    borderColor: color,
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: {
                    x: { ticks: { autoSkip: false, maxRotation: 35, font: { size: 11 } } },
                    y: { min: 0, max: 100, ticks: { callback: v => v + '%' } }
                },
                onClick(event) {
                    const hits = groupedChart.getElementsAtEventForMode(
                        event, 'nearest', { intersect: true }, false
                    );
                    if (!hits.length) return;
                    const { index } = hits[0];
                    const ind = aspect.indicators[index];
                    showAssessmentItems(assessPanelId, ind, ind.name, aspect.name, d.name);
                }
            }
        });
    }

    function activatePill(aspectIdx) {
        const allPills = panel.querySelectorAll('.heat-level-pill');
        allPills.forEach(p => {
            const i = parseInt(p.dataset.aspectIdx);
            const c = pillColors[i] || '#888';
            Object.assign(p.style, {
                background: c + '22', color: c,
                border: `1.5px solid ${c}`,
                fontWeight: '600', boxShadow: ''
            });
        });
        const activePill = panel.querySelector(`.heat-level-pill[data-aspect-idx="${aspectIdx}"]`);
        if (activePill) {
            const c = pillColors[aspectIdx] || '#888';
            Object.assign(activePill.style, {
                background: c, color: '#fff',
                border: `1.5px solid ${c}`,
                fontWeight: '700', boxShadow: `0 2px 8px ${c}88`
            });
        }
    }

    if (hasAspects) {
        panel.querySelectorAll('.heat-level-pill').forEach(pill => {
            pill.addEventListener('click', function () {
                const idx = parseInt(this.dataset.aspectIdx);
                activatePill(idx);
                applyHeatmapFilter(idx);
                renderAspectChart(idx);
            });
        });

        // BYDefault: show first aspect on load
        activatePill(0);
        applyHeatmapFilter(0);
        renderAspectChart(0);
    }
}


function showAssessmentItems(panelId, indicator, indicatorName, aspectName, intelligenceName) {
    const panel = document.getElementById(panelId);
    if (!panel) return;

    if (!indicator || !indicator.assessmentItems || !indicator.assessmentItems.length) {
        panel.innerHTML = `
            <div class="alert alert-light border text-muted" style="font-size:13px">
                لا تتوفر بنود تقييم لمؤشر "<strong>${indicatorName}</strong>".
            </div>`;
        return;
    }

    const rows = indicator.assessmentItems.map(item => {
        const pct = Math.round(item.finalScore * 100);
        return `
            <tr>
                <td style="font-size:13px">${item.itemName}</td>
                <td style="text-align:center;font-size:13px">${pct}%</td>
                <td style="text-align:center;font-size:13px">${item.psychometricPts}</td>
                <td style="text-align:center">
                    <span style="${ratingBadgeStyle(item.rating)}">${item.rating}</span>
                </td>
            </tr>`;
    }).join("");

    panel.innerHTML = `
        <div class="card border-0 border-top shadow-sm" style="border-top:3px solid #378ADD !important">
            <div class="card-body">
                <p class="mb-1 fw-semibold" style="font-size:13px">
                    تفاصيل قياس مؤشر:
                    <span style="color:#378ADD">${indicatorName}</span>
                    &nbsp;—&nbsp;
                    <span class="text-muted">الجانب: ${aspectName}</span>
                    &nbsp;—&nbsp;
                    <span class="text-muted">الذكاء: ${intelligenceName}</span>
                </p>
                <div class="table-responsive mt-3">
                    <table class="dash-table table-sm table-hover align-middle mb-0">
                        <thead class="dash-thead-yellow">
                            <tr>
                                <th>اسم البند</th>
                                <th style="text-align:center">الدرجة النهائية</th>
                                <th style="text-align:center">النقاط السيكومترية</th>
                                <th style="text-align:center">التقدير</th>
                            </tr>
                        </thead>
                        <tbody>${rows}</tbody>
                    </table>
                </div>
            </div>
        </div>`;

    panel.scrollIntoView({ behavior: "smooth", block: "nearest" });
}

function showEmptyState(canvas, message) {
    canvas.style.display = "none";
    const msg = document.createElement("p");
    msg.className = "text-muted text-center py-4";
    msg.style.fontSize = "13px";
    msg.textContent = message;
    canvas.parentNode.insertBefore(msg, canvas);
}