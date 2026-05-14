import{a as Se}from"./chunk-RR3MGKCW.js";import{a as be}from"./chunk-PDHLZSH3.js";import{a as le}from"./chunk-EAZFAJKX.js";import{a as U,b as J,c as X}from"./chunk-RYELYLUJ.js";import{a as he}from"./chunk-KLIEBLUE.js";import{a as pe,b as de}from"./chunk-OJNJNY6A.js";import{a as oe,b as se}from"./chunk-6YXLEAB2.js";import{b as ee,e as te,g as ie,m as ne,o as ae}from"./chunk-K3VG2N5A.js";import{a as ye,b as Ce}from"./chunk-4Z7TX5CY.js";import{b as ue}from"./chunk-KPRAZYGR.js";import{a as ve,b as xe}from"./chunk-YQGGV7IP.js";import{a as _e}from"./chunk-S6CSFVPE.js";import{a as fe}from"./chunk-4BVXDFJU.js";import{a as ge}from"./chunk-GPO4JYBW.js";import{a as me}from"./chunk-SWJ6TRC3.js";import{d as re,e as ce}from"./chunk-SOOB26PC.js";import{e as Y}from"./chunk-ODDPWDE5.js";import{p as G}from"./chunk-OC4L6P6A.js";import{a as K}from"./chunk-JZJWV6OM.js";import"./chunk-GKCTFK3Y.js";import"./chunk-VTUVH3A7.js";import"./chunk-ZF5EHKAH.js";import{Ab as r,Ac as P,Bb as y,Cb as S,Db as L,Eb as $,Fb as O,Gb as k,Ib as q,Ja as s,Jb as Q,M as F,N as H,S as _,Sb as T,Tb as w,Tc as j,Ua as I,Uc as V,X as R,Y as z,_a as p,_c as Z,ab as C,cb as N,fa as u,fc as E,ga as f,gb as g,hb as W,ib as D,jb as A,kb as i,lb as n,mb as d,qb as b,rb as x,sb as l}from"./chunk-DY4L5YQN.js";import"./chunk-LGPK3X4Q.js";var Te=(()=>{class t{constructor(){this.api=_(me)}processPayment(e){return this.api.post("/payments",e)}getPaymentsByOrder(e){return this.api.get(`/payments/by-order/${e}`)}getPaymentMethods(){return this.api.get("/payments/methods")}static{this.\u0275fac=function(o){return new(o||t)}}static{this.\u0275prov=F({token:t,factory:t.\u0275fac,providedIn:"root"})}}return t})();var we=(()=>{class t{constructor(){this.companySettings=_(Se),this.lastReceipt=null}printReceipt(e){this.lastReceipt=e;let o=this.buildReceiptHtml(e),a=window.open("","_blank","width=400,height=700,scrollbars=yes");a&&(a.document.write(o),a.document.close(),a.onload=()=>{a.focus(),a.print()})}reprintLast(){this.lastReceipt&&this.printReceipt(this.lastReceipt)}hasLastReceipt(){return this.lastReceipt!==null}fmt(e){return`L ${e.toFixed(2)}`}buildReceiptHtml(e){let o=new Date,a=o.toLocaleDateString("es-HN",{day:"2-digit",month:"2-digit",year:"numeric"}),m=o.toLocaleTimeString("es-HN",{hour:"2-digit",minute:"2-digit"}),v=e.order,h=this.companySettings.settings(),Ee=v.items.map(M=>{let Oe=M.modifiers?.length?`<div class="mods">${M.modifiers.map(Pe=>`  + ${Pe.name}`).join("<br>")}</div>`:"",ke=M.notes?`<div class="mods">  * ${M.notes}</div>`:"";return`
        <tr>
          <td class="qty">${M.quantity}</td>
          <td class="desc">
            ${M.productName}
            ${Oe}${ke}
          </td>
          <td class="price">${this.fmt(M.subtotal)}</td>
        </tr>`}).join(""),Fe=v.tableNumber?`<div class="info-row"><span>Mesa</span><span>${v.tableNumber}</span></div>`:"",De=e.discountAmount>0?`<tr class="discount-row">
           <td colspan="2">Descuento (${v.discountPercent}%)</td>
           <td>-${this.fmt(e.discountAmount)}</td>
         </tr>`:"",Ae=e.paymentMethod==="Efectivo"&&e.change!==void 0&&e.change>0?`<div class="info-row"><span>Efectivo recibido</span><span>${this.fmt(e.receivedAmount??0)}</span></div>
         <div class="info-row cambio"><span>CAMBIO</span><span>${this.fmt(e.change)}</span></div>`:"",$e=e.reference?`<div class="info-row"><span>Referencia</span><span>${e.reference}</span></div>`:"";return`<!DOCTYPE html>
<html lang="es">
<head>
<meta charset="UTF-8">
<title>Recibo ${v.orderNumber}</title>
<style>
  * { margin: 0; padding: 0; box-sizing: border-box; }

  body {
    font-family: 'Courier New', Courier, monospace;
    font-size: 12px;
    color: #000;
    background: #fff;
    width: 80mm;
    margin: 0 auto;
    padding: 4mm 3mm;
  }

  .center { text-align: center; }
  .bold   { font-weight: bold; }
  .divider { border-top: 1px dashed #000; margin: 4px 0; }

  .header { text-align: center; margin-bottom: 6px; }
  .header .name { font-size: 16px; font-weight: bold; letter-spacing: 1px; }
  .header .sub  { font-size: 10px; color: #333; margin-top: 2px; }

  .info-row {
    display: flex;
    justify-content: space-between;
    font-size: 11px;
    margin: 2px 0;
  }
  .info-row.cambio { font-weight: bold; font-size: 13px; }

  table {
    width: 100%;
    border-collapse: collapse;
    margin: 4px 0;
    font-size: 11px;
  }
  td { vertical-align: top; padding: 2px 0; }
  td.qty   { width: 20px; text-align: center; }
  td.desc  { padding-left: 4px; }
  td.price { text-align: right; white-space: nowrap; }
  .mods { font-size: 10px; color: #444; }

  .discount-row td { color: #333; }

  .totals { margin-top: 4px; }
  .totals .info-row { font-size: 12px; }
  .totals .info-row.total-final {
    font-size: 15px;
    font-weight: bold;
    border-top: 1px solid #000;
    padding-top: 3px;
    margin-top: 3px;
  }

  .footer {
    text-align: center;
    font-size: 10px;
    margin-top: 8px;
    color: #555;
  }

  @media print {
    body { width: 80mm; margin: 0; padding: 2mm; }
    @page { margin: 0; size: 80mm auto; }
  }
</style>
</head>
<body>

  <div class="header">
    <div class="name">${h.name}</div>
    ${h.address?`<div class="sub">${h.address}</div>`:""}
    ${h.city?`<div class="sub">${h.city}</div>`:""}
    ${h.phone?`<div class="sub">Tel: ${h.phone}</div>`:""}
    ${h.rtn?`<div class="sub">RTN: ${h.rtn}</div>`:""}
  </div>

  <div class="divider"></div>

  <div class="info-row"><span>Fecha</span><span>${a}</span></div>
  <div class="info-row"><span>Hora</span><span>${m}</span></div>
  <div class="info-row bold"><span>Pedido</span><span>${v.orderNumber}</span></div>
  ${Fe}
  <div class="info-row"><span>Atendido por</span><span>${v.waiterName}</span></div>

  <div class="divider"></div>

  <table>
    <thead>
      <tr>
        <td class="qty bold">#</td>
        <td class="desc bold">Descripci\xF3n</td>
        <td class="price bold">Precio</td>
      </tr>
    </thead>
    <tbody>
      ${Ee}
      ${De}
    </tbody>
  </table>

  <div class="divider"></div>

  <div class="totals">
    <div class="info-row"><span>Subtotal</span><span>${this.fmt(v.subtotal)}</span></div>
    ${e.discountAmount>0?`<div class="info-row"><span>Descuento (${v.discountPercent}%)</span><span>-${this.fmt(e.discountAmount)}</span></div>`:""}
    <!-- <div class="info-row"><span>ISV (15%)</span><span>${this.fmt(e.taxAmount)}</span></div> -->
    <div class="info-row total-final"><span>TOTAL</span><span>${this.fmt(e.finalTotal)}</span></div>
  </div>

  <div class="divider"></div>

  <div class="info-row bold"><span>M\xE9todo de pago</span><span>${e.paymentMethod}</span></div>
  ${Ae}
  ${$e}

  <div class="divider"></div>

  <div class="footer">
    <div>${h.receiptFooter}</div>
    <div style="margin-top:4px; font-size:9px;">
      Este recibo no es una factura fiscal.<br>
      Para factura con RTN solic\xEDtela al cajero.
    </div>
  </div>

</body>
</html>`}static{this.\u0275fac=function(o){return new(o||t)}}static{this.\u0275prov=F({token:t,factory:t.\u0275fac,providedIn:"root"})}}return t})();var Me=(()=>{class t{static \u0275fac=function(o){return new(o||t)};static \u0275mod=z({type:t});static \u0275inj=H({imports:[P,Z,G,J,X,U,K,V,V]})}return t})();var Ie=(t,c)=>c.id,Re=()=>["Efectivo","Tarjeta","Transferencia"];function Ne(t,c){if(t&1){let e=b();i(0,"button",16),x("click",function(){u(e);let a=l();return f(a.receiptService.reprintLast())}),d(1,"i",17),i(2,"span",7),r(3,"Reimprimir"),n()()}t&2&&C("text",!0)}function Le(t,c){t&1&&(i(0,"div",10),d(1,"i",18),i(2,"span"),r(3,"No hay sesi\xF3n de caja activa. Ve a "),i(4,"a",19),r(5,"Sesi\xF3n de Caja"),n(),r(6," para abrir una caja antes de cobrar. "),n()())}function je(t,c){t&1&&(i(0,"div",20)(1,"span",21),r(2,"Pedidos Listos para Cobrar"),n()())}function Ve(t,c){t&1&&(i(0,"div",14),d(1,"i",22),n())}function Be(t,c){t&1&&(i(0,"div",23),d(1,"i",24),i(2,"h3"),r(3,"Todo cobrado"),n(),i(4,"p"),r(5,"No hay pedidos pendientes de pago"),n()())}function He(t,c){if(t&1&&(i(0,"span",30),r(1),n()),t&2){let e=l().$implicit;s(),S("Mesa ",e.tableNumber,"")}}function ze(t,c){if(t&1){let e=b();i(0,"div",27),x("click",function(){let a=u(e).$implicit,m=l(2);return f(m.selectOrder(a))}),i(1,"div",28)(2,"span",29),r(3),n(),p(4,He,2,1,"span",30),i(5,"span",31),r(6),n()(),i(7,"div",32)(8,"span",33),r(9),T(10,"posCurrency"),n(),i(11,"small",34),r(12),n()()()}if(t&2){let e,o=c.$implicit,a=l(2);N("selected",((e=a.selectedOrder())==null?null:e.id)===o.id),s(3),S("#",o.orderNumber,""),s(),g(4,o.tableNumber?4:-1),s(2),y(o.waiterName),s(3),y(w(10,8,o.total)),s(3),L("",o.items.length," \xEDtem",o.items.length!==1?"s":"","")}}function We(t,c){if(t&1&&(i(0,"div",25),D(1,ze,13,10,"div",26,Ie),n()),t&2){let e=l();s(),A(e.orders())}}function qe(t,c){if(t&1&&(i(0,"span",30),r(1),n()),t&2){let e=l(3);s(),S("Mesa ",e.selectedOrder().tableNumber,"")}}function Qe(t,c){if(t&1&&(i(0,"div",49),d(1,"i",50),i(2,"span",33),r(3),n(),p(4,qe,2,1,"span",30),n()),t&2){let e=l(2);s(3),S("Cobrar #",e.selectedOrder().orderNumber,""),s(),g(4,e.selectedOrder().tableNumber?4:-1)}}function Ye(t,c){if(t&1&&(i(0,"div",36)(1,"span"),r(2),n(),i(3,"span",51),r(4),T(5,"posCurrency"),n()()),t&2){let e=c.$implicit;s(2),L("",e.quantity,"\xD7 ",e.productName,""),s(2),y(w(5,3,e.subtotal))}}function Ge(t,c){if(t&1&&(i(0,"div",42)(1,"span"),r(2),n(),i(3,"span"),r(4),T(5,"posCurrency"),n()()),t&2){let e=l(2);s(2),S("Descuento (",e.paymentForm.discountPercent,"%)"),s(2),S("\u2212",w(5,2,e.discountAmount()),"")}}function Ue(t,c){if(t&1){let e=b();i(0,"button",52),x("click",function(){let a=u(e).$implicit,m=l(2);return m.setPaymentMethod(a),f(m.recalculate())}),r(1),n()}if(t&2){let e=c.$implicit,o=l(2);N("active",o.paymentForm.method===e),s(),y(e)}}function Je(t,c){if(t&1&&(i(0,"div",54)(1,"span",55),r(2,"Cambio a devolver"),n(),i(3,"span",56),r(4),T(5,"posCurrency"),n()()),t&2){let e=l(3);s(4),y(w(5,1,e.change()))}}function Ke(t,c){if(t&1){let e=b();i(0,"div",46)(1,"label",21),r(2,"Efectivo recibido"),n(),i(3,"p-inputNumber",53),k("ngModelChange",function(a){u(e);let m=l(2);return O(m.paymentForm.receivedAmount,a)||(m.paymentForm.receivedAmount=a),f(a)}),x("ngModelChange",function(){u(e);let a=l(2);return f(a.recalculate())}),n()(),p(4,Je,6,3,"div",54)}if(t&2){let e=l(2);s(3),$("ngModel",e.paymentForm.receivedAmount),C("min",e.finalTotal())("minFractionDigits",2)("maxFractionDigits",2),s(),g(4,e.change()>0?4:-1)}}function Xe(t,c){if(t&1){let e=b();i(0,"div",46)(1,"label",21),r(2,"Referencia / \xDAltimos 4 d\xEDgitos"),n(),i(3,"input",57),k("ngModelChange",function(a){u(e);let m=l(2);return O(m.paymentForm.cardReference,a)||(m.paymentForm.cardReference=a),f(a)}),n()()}if(t&2){let e=l(2);s(3),$("ngModel",e.paymentForm.cardReference)}}function Ze(t,c){if(t&1){let e=b();i(0,"p-card"),p(1,Qe,5,2,"ng-template",13),i(2,"div",0)(3,"div",35),D(4,Ye,6,5,"div",36,Ie),n(),i(6,"div",37)(7,"label",38),r(8,"Descuento (%)"),n(),i(9,"p-inputNumber",39),k("ngModelChange",function(a){u(e);let m=l();return O(m.paymentForm.discountPercent,a)||(m.paymentForm.discountPercent=a),f(a)}),x("ngModelChange",function(){u(e);let a=l();return f(a.recalculate())}),n()(),i(10,"div",40)(11,"div",41)(12,"span"),r(13,"Subtotal"),n(),i(14,"span"),r(15),T(16,"posCurrency"),n()(),p(17,Ge,6,4,"div",42),d(18,"p-divider",43),i(19,"div",44)(20,"span"),r(21,"TOTAL A COBRAR"),n(),i(22,"span"),r(23),T(24,"posCurrency"),n()()(),i(25,"div",25)(26,"label",21),r(27,"M\xE9todo de Pago"),n(),i(28,"div",3),D(29,Ue,2,3,"button",45,W),n()(),p(31,Ke,5,5)(32,Xe,4,1,"div",46),i(33,"button",47),x("click",function(){u(e);let a=l();return f(a.processPayment())}),d(34,"i",48),i(35,"span",7),r(36),T(37,"posCurrency"),n()()()()}if(t&2){let e=l();s(4),A(e.selectedOrder().items),s(5),$("ngModel",e.paymentForm.discountPercent),C("min",0)("max",100)("step",1),s(6),y(w(16,12,e.selectedOrder().subtotal)),s(2),g(17,e.paymentForm.discountPercent>0?17:-1),s(6),y(w(24,14,e.finalTotal())),s(6),A(Q(18,Re)),s(2),g(31,e.paymentForm.method==="Efectivo"?31:-1),s(),g(32,e.paymentForm.method==="Tarjeta"?32:-1),s(),C("loading",e.paying())("disabled",!e.canPay()),s(3),S("Cobrar ",w(37,16,e.finalTotal()),"")}}function et(t,c){t&1&&(i(0,"p-card")(1,"div",23),d(2,"i",58),i(3,"h3"),r(4,"Selecciona un pedido"),n(),i(5,"p"),r(6,"Haz clic en un pedido de la lista para cobrar"),n()()())}var tt={Efectivo:"CASH",Tarjeta:"CARD",Transferencia:"TRANSFER"},mi=(()=>{class t{constructor(){this.ordersService=_(fe),this.paymentService=_(Te),this.cashRegisterService=_(be),this.notifications=_(_e),this.receiptService=_(we),this.orders=I([]),this.selectedOrder=I(null),this.activeSession=I(null),this.loading=I(!1),this.paying=I(!1),this.paymentForm={method:"Efectivo",cashAmount:0,cardAmount:0,cardReference:"",receivedAmount:0,discountPercent:0},this.discountAmount=E(()=>{let e=this.selectedOrder();return e?e.subtotal*(this.paymentForm.discountPercent/100):0}),this.taxAmount=E(()=>{let e=this.selectedOrder();return e?(e.subtotal-this.discountAmount())*.15:0}),this.finalTotal=E(()=>{let e=this.selectedOrder();return e?e.subtotal-this.discountAmount()+this.taxAmount():0}),this.change=E(()=>this.paymentForm.method!=="Efectivo"?0:Math.max(0,this.paymentForm.receivedAmount-this.finalTotal())),this.canPay=E(()=>this.selectedOrder()?this.paymentForm.method==="Efectivo"?this.paymentForm.receivedAmount>=this.finalTotal():!0:!1)}ngOnInit(){this.loadOrders(),this.loadSession()}loadOrders(){this.loading.set(!0),this.ordersService.getActiveOrders().subscribe({next:e=>{this.orders.set(e.filter(o=>o.status==="Listo"||o.status==="Entregado")),this.loading.set(!1)},error:()=>this.loading.set(!1)})}loadSession(){this.cashRegisterService.getMyActiveSession().subscribe({next:e=>this.activeSession.set(e),error:()=>{}})}selectOrder(e){this.selectedOrder.set(e),this.paymentForm={method:"Efectivo",cashAmount:0,cardAmount:0,cardReference:"",receivedAmount:e.total,discountPercent:0}}recalculate(){this.paymentForm.method==="Efectivo"&&this.paymentForm.receivedAmount<this.finalTotal()&&(this.paymentForm.receivedAmount=this.finalTotal())}processPayment(){let e=this.selectedOrder();if(!e)return;this.paying.set(!0);let o={orderId:e.id,paymentMethodCode:tt[this.paymentForm.method]??this.paymentForm.method,amount:this.finalTotal(),discountPercent:this.paymentForm.discountPercent>0?this.paymentForm.discountPercent:void 0,receivedAmount:this.paymentForm.method==="Efectivo"?this.paymentForm.receivedAmount:void 0,reference:this.paymentForm.method==="Tarjeta"?this.paymentForm.cardReference:void 0},a={order:e,paymentMethod:this.paymentForm.method,finalTotal:this.finalTotal(),discountAmount:this.discountAmount(),taxAmount:this.taxAmount(),receivedAmount:this.paymentForm.method==="Efectivo"?this.paymentForm.receivedAmount:void 0,change:this.paymentForm.method==="Efectivo"?this.change():void 0,reference:this.paymentForm.method==="Tarjeta"?this.paymentForm.cardReference:void 0};this.paymentService.processPayment(o).subscribe({next:()=>{this.paying.set(!1),this.notifications.success("Pago procesado",`Pedido #${e.orderNumber} cobrado exitosamente`),this.receiptService.printReceipt(a),this.selectedOrder.set(null),this.loadOrders()},error:()=>{this.paying.set(!1),this.notifications.error("Error","No se pudo procesar el pago")}})}setPaymentMethod(e){this.paymentForm.method=e}static{this.\u0275fac=function(o){return new(o||t)}}static{this.\u0275cmp=R({type:t,selectors:[["app-cashier-dashboard"]],standalone:!0,features:[q],decls:23,vars:6,consts:[[1,"flex","flex-column","gap-3"],[1,"flex","align-items-center","justify-content-between","flex-wrap","gap-2"],[1,"m-0"],[1,"flex","gap-2"],["pButton","","severity","secondary","size","small",3,"text"],["pButton","","severity","secondary","size","small","routerLink","/cashier/session",3,"text"],[1,"pi","pi-wallet","pButtonIcon"],["pButtonLabel",""],["pButton","","severity","secondary","size","small","pTooltip","Actualizar lista",3,"click","text"],[1,"pi","pi-refresh","pButtonIcon"],[1,"warning-banner"],[1,"grid",2,"align-items","start"],[1,"col-12","md:col-7"],["pTemplate","header"],[1,"flex","justify-content-center","p-5"],[1,"col-12","md:col-5"],["pButton","","severity","secondary","size","small",3,"click","text"],[1,"pi","pi-print","pButtonIcon"],[1,"pi","pi-exclamation-triangle"],["routerLink","/cashier/session"],[1,"px-3","pt-3","pb-1"],[1,"text-xs","font-bold","text-color-secondary","uppercase"],[1,"pi","pi-spin","pi-spinner","text-4xl","text-color-secondary"],[1,"empty-state"],[1,"pi","pi-check-circle","empty-icon","text-green-400"],[1,"flex","flex-column","gap-2"],[1,"order-row",3,"selected"],[1,"order-row",3,"click"],[1,"flex","align-items-center","gap-3"],[1,"font-bold","text-accent"],[1,"badge","badge-primary"],[1,"text-xs","text-muted"],[1,"flex","flex-column","align-items-end","gap-1"],[1,"font-bold"],[1,"text-muted"],[1,"cart-scroll",2,"max-height","140px","background","rgba(255,255,255,0.025)","border-radius","8px","padding","0.5rem 0.75rem"],[1,"flex","justify-content-between","text-sm","text-color-secondary","py-1"],[1,"flex","align-items-center","gap-2"],[1,"text-sm","text-color-secondary","font-semibold","white-space-nowrap"],["inputStyleClass","w-full","styleClass","flex-1",3,"ngModelChange","ngModel","min","max","step"],[1,"summary-card__totals","pos-card",2,"padding","0.875rem 1rem"],[1,"total-row"],[1,"total-row",2,"color","#4ade80"],["styleClass","my-1"],[1,"total-row","total-row--main"],[1,"payment-tab",3,"active"],[1,"flex","flex-column","gap-1"],["pButton","","severity","success",1,"w-full",2,"height","48px","font-size","0.95rem",3,"click","loading","disabled"],[1,"pi","pi-check-circle","pButtonIcon"],[1,"px-3","pt-3","pb-1","flex","align-items-center","gap-2"],[1,"pi","pi-receipt","text-color-secondary"],[1,"font-semibold"],[1,"payment-tab",3,"click"],["inputStyleClass","w-full text-xl font-bold text-center",1,"w-full",3,"ngModelChange","ngModel","min","minFractionDigits","maxFractionDigits"],[1,"change-display"],[1,"change-display__label"],[1,"change-display__amount"],["pInputText","","placeholder","Ej: 4521","maxlength","20",1,"w-full",3,"ngModelChange","ngModel"],[1,"pi","pi-hand-pointer","empty-icon"]],template:function(o,a){o&1&&(i(0,"div",0)(1,"div",1)(2,"h2",2),r(3,"Caja"),n(),i(4,"div",3),p(5,Ne,4,1,"button",4),i(6,"button",5),d(7,"i",6),i(8,"span",7),r(9,"Sesi\xF3n de Caja"),n()(),i(10,"button",8),x("click",function(){return a.loadOrders()}),d(11,"i",9),n()()(),p(12,Le,7,0,"div",10),i(13,"div",11)(14,"div",12)(15,"p-card"),p(16,je,3,0,"ng-template",13)(17,Ve,2,0,"div",14)(18,Be,6,0)(19,We,3,0),n()(),i(20,"div",15),p(21,Ze,38,19,"p-card")(22,et,7,0),n()()()),o&2&&(s(5),g(5,a.receiptService.hasLastReceipt()?5:-1),s(),C("text",!0),s(4),C("text",!0),s(2),g(12,a.activeSession()?-1:12),s(5),g(17,a.loading()?17:a.orders().length===0?18:19),s(4),g(21,a.selectedOrder()?21:22))},dependencies:[P,Y,ae,ee,te,ne,ie,ge,ce,re,j,de,pe,se,oe,Ce,ye,xe,ve,ue,he,le,Me],encapsulation:2})}}return t})();export{mi as CashierDashboardComponent};
