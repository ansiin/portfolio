const EURO = "\u20AC";

const money = (value) => {
  if (value === null || value === undefined || Number.isNaN(value)) {
    return "N/A";
  }
  return `${value.toFixed(2)}${EURO}`;
};

const profitClass = (value) => {
  if (value === null || value === undefined || Number.isNaN(value)) {
    return "neutral";
  }
  if (value > 0) {
    return "positive";
  }
  if (value < 0) {
    return "negative";
  }
  return "neutral";
};

function renderCards(current) {
  const container = document.getElementById("currentCards");
  container.innerHTML = "";

  Object.values(current).forEach((item) => {
    const card = document.createElement("article");
    card.className = "stat-card";
    card.innerHTML = `
      <h3>${item.label}</h3>
      <div class="metric-row"><span class="metric-label">Kogus</span><span class="metric-value">${item.quantity}</span></div>
      <div class="metric-row"><span class="metric-label">Ostuhind</span><span class="metric-value">${money(item.baselinePrice)}</span></div>
      <div class="metric-row"><span class="metric-label">Hetkehind</span><span class="metric-value">${money(item.currentPrice)}</span></div>
      <div class="metric-row"><span class="metric-label">Kasum</span><span class="metric-value ${profitClass(item.profit)}">${money(item.profit)}</span></div>
    `;
    container.appendChild(card);
  });
}

function renderTable(history) {
  const body = document.getElementById("historyTable");
  body.innerHTML = "";

  history.slice().reverse().forEach((entry) => {
    const row = document.createElement("tr");
    row.innerHTML = `
      <td>${entry.label}</td>
      <td>${money(entry.glovesPrice)}</td>
      <td>${money(entry.spectrumPrice)}</td>
      <td class="${profitClass(entry.glovesProfit)}">${money(entry.glovesProfit)}</td>
      <td class="${profitClass(entry.spectrumProfit)}">${money(entry.spectrumProfit)}</td>
    `;
    body.appendChild(row);
  });
}

function createSvgNode(tag, attrs = {}) {
  const node = document.createElementNS("http://www.w3.org/2000/svg", tag);
  Object.entries(attrs).forEach(([key, value]) => node.setAttribute(key, value));
  return node;
}

function drawLineChart(svg, series) {
  const width = 900;
  const height = Number(svg.getAttribute("viewBox").split(" ")[3]);
  const margin = { top: 18, right: 18, bottom: 34, left: 56 };
  svg.innerHTML = "";

  const points = series.flatMap((item) => item.values).filter((value) => value !== null);
  if (points.length === 0) {
    svg.appendChild(createSvgNode("text", { x: 36, y: 42, fill: "#96a7c5" })).textContent = "Andmed puuduvad";
    return;
  }

  const min = Math.min(...points);
  const max = Math.max(...points);
  const range = max - min || 1;
  const length = Math.max(series[0]?.values.length ?? 0, 1);

  const xFor = (index) => margin.left + ((width - margin.left - margin.right) * index) / Math.max(length - 1, 1);
  const yFor = (value) => height - margin.bottom - ((value - min) / range) * (height - margin.top - margin.bottom);

  for (let i = 0; i < 4; i += 1) {
    const y = margin.top + ((height - margin.top - margin.bottom) * i) / 3;
    svg.appendChild(createSvgNode("line", {
      x1: margin.left,
      y1: y,
      x2: width - margin.right,
      y2: y,
      stroke: "rgba(150, 167, 197, 0.16)",
      "stroke-width": "1",
    }));
  }

  for (let i = 0; i <= 3; i += 1) {
    const value = max - (range * i) / 3;
    const y = margin.top + ((height - margin.top - margin.bottom) * i) / 3;
    const label = createSvgNode("text", {
      x: 8,
      y: y + 4,
      fill: "#96a7c5",
      "font-size": "12",
    });
    label.textContent = `${value.toFixed(2)}${EURO}`;
    svg.appendChild(label);
  }

  const labels = series[0]?.labels ?? [];
  const step = Math.max(Math.ceil(labels.length / 6), 1);
  labels.forEach((label, index) => {
    if (index % step !== 0 && index !== labels.length - 1) {
      return;
    }
    const text = createSvgNode("text", {
      x: xFor(index),
      y: height - 10,
      fill: "#96a7c5",
      "font-size": "12",
      "text-anchor": "middle",
    });
    text.textContent = label;
    svg.appendChild(text);
  });

  series.forEach((item) => {
    const pathParts = [];
    item.values.forEach((value, index) => {
      if (value === null) {
        return;
      }
      pathParts.push(`${pathParts.length === 0 ? "M" : "L"} ${xFor(index)} ${yFor(value)}`);
    });

    svg.appendChild(createSvgNode("path", {
      d: pathParts.join(" "),
      fill: "none",
      stroke: item.color,
      "stroke-width": "4",
      "stroke-linecap": "round",
      "stroke-linejoin": "round",
    }));

    item.values.forEach((value, index) => {
      if (value === null) {
        return;
      }
      svg.appendChild(createSvgNode("circle", {
        cx: xFor(index),
        cy: yFor(value),
        r: "4",
        fill: item.color,
      }));
    });
  });
}

function renderCharts(history) {
  const labels = history.map((entry) => entry.label);
  drawLineChart(document.getElementById("priceChart"), [
    { labels, values: history.map((entry) => entry.glovesPrice), color: "#6ae3c0" },
    { labels, values: history.map((entry) => entry.spectrumPrice), color: "#ff9f6e" },
  ]);

  drawLineChart(document.getElementById("profitChart"), [
    { labels, values: history.map((entry) => entry.glovesProfit), color: "#85a9ff" },
    { labels, values: history.map((entry) => entry.spectrumProfit), color: "#ff9f6e" },
  ]);
}

async function loadDashboard() {
  const status = document.getElementById("statusLine");
  const button = document.getElementById("refreshButton");
  button.disabled = true;
  status.textContent = "Laen andmeid...";

  try {
    const response = await fetch("/api/dashboard", { cache: "no-store" });
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }
    const payload = await response.json();
    renderCards(payload.current);
    renderTable(payload.history);
    renderCharts(payload.history);
    status.textContent = `Uuendatud: ${payload.lastUpdated}`;
  } catch (error) {
    status.textContent = `Viga: ${error.message}`;
  } finally {
    button.disabled = false;
  }
}

document.getElementById("refreshButton").addEventListener("click", loadDashboard);
loadDashboard();
