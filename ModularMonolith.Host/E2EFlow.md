# Features

Resumen paso a paso:

-- Primera Etapa -- 

1. Autenticación / Identidad
   - Usuario se registra / inicia sesión (Identity). Soporta username/password, proveedores OAuth y emisión de JWT o cookies según el cliente.
   - Gestión de roles y claims (cliente, admin, operador).

2. Navegación y catálogo
   - Consulta del catálogo (Catalog): búsqueda, filtrado, variantes, precios y disponibilidad.
   - Visualización de producto (opcional: evento ProductViewed para analítica).



3. Carrito y sesión
   - Añadir items al carrito (ShoppingCart/Checkout). Carrito sincronizado cliente <-> servidor; persistido si el usuario está autenticado.
   - Cálculo de totales preliminares (impuestos, descuentos, tarifas de envío estimadas).

-- Primera Etapa --

4. Checkout
   - Recopilación de direcciones y selección de método de envío (Shipping).
   - Aplicación de promociones/cupones (Promotions) y re-cálculo de totales.
   - Selección del método de pago.

5. Reserva y creación de pedido
   - Creación de un pedido en estado inicial (Ordering) y intento de reservar inventario (Inventory).
   - Si la reserva falla, informar y bloquear la confirmación hasta resolver.

6. Pago
   - Payments envía autorización al proveedor (Stripe/PayPal u otro) usando tokenización; no almacenar datos de tarjeta.
   - Soporte de flujos síncronos y asíncronos (p. ej. 3DS, webhooks).
   - Uso de idempotency keys para evitar cobros duplicados.

7. Confirmación y procesos posteriores
   - Si el pago se autoriza/captura: el pedido pasa a confirmado; emitir eventos OrderPlaced / PaymentCaptured.
   - Confirmar y reducir inventario, crear envío (Shipping) y notificar al cliente (Notifications: email/SMS).
   - Si el pago falla: marcar el pedido como fallido o en espera y liberar reservas si procede.

8. Fulfillment y tracking
   - Generar etiquetas/envíos, actualizar estados (Shipped, Delivered) y notificar al cliente.
   - Soporte para devoluciones y reembolsos (coordinar Payments + Ordering).

9. Operaciones administrativas
   - Admin: panel para gestionar productos, pedidos, clientes, promociones y métricas.
   - Reconciliación de pagos, gestión de reembolsos y auditoría.

10. Mantenimiento y background
    - Tareas en background (BackgroundJobs): reintentos de webhooks, limpieza de carritos inactivos, reconciliación, generación de reportes.

---

# Patrones y consideraciones clave

- Modularización: separar responsabilidades por módulo con sus propios límites y DbContext cuando sea apropiado (Catalog, Ordering, Payments, Inventory, Identity, Shipping, Promotions, Notifications, Admin, BackgroundJobs).
- Consistencia: usar transacciones locales para acciones atómicas; para procesos compuestos usar eventos, sagas o mecanismos de compensación para eventual consistency.
- Seguridad y cumplimiento: tokenizar datos sensibles, no almacenar PAN, cifrar secretos y cumplir requisitos PCI según el proveedor de pagos.
- Fiabilidad: idempotencia, manejo robusto de webhooks, reintentos exponenciales, logging y telemetría.
- Observabilidad: emitir eventos de dominio, métricas y trazas distribuidas para depuración y monitoreo.

---

# Módulos sugeridos (priorizados)

## Mínimo viable (imprescindibles):
- **Identity**: autenticación, autorización y gestión de usuarios/roles.
- **Catalog**: productos, categorías, variantes y precios.
- **ShoppingCart / Checkout**: gestión del carrito y flujo de compra.
- **Ordering**: modelo de pedido, estados y historial.
- **Payments**: integración con pasarelas, webhooks y reembolsos.
- **Inventory**: stock, reservas y liberaciones.
- **Notifications**: envío de emails/SMS y plantillas.
- **Admin**: APIs y UI para gestión operativa.

## Alta prioridad:
- **Shipping**: cálculo de tarifas, integración con transportistas y generación de envíos/trackings.
- **Promotions / Pricing**: reglas de descuento y campañas.
- **Media / FileStorage**: almacenamiento de imágenes y recursos (S3/Blob/local).
- **BackgroundJobs**: colas y workers para tareas asíncronas.

## Baja prioridad (mejoras u operativa):
- **Search / Indexing**: ElasticSearch o similar para búsquedas avanzadas.
- **Reporting / Analytics**: paneles y exportación de KPI.
- **Customer / Profile**: historial, direcciones guardadas y métodos de pago tokenizados.
- **Billing / Accounting**: facturación y conciliación.
- **Integration / Gateway**: adaptadores para terceros (ERP, CRM).
- **Audit / Compliance**: trazabilidad y logs de acciones administrativas.

---

# Módulos faltantes respecto al plan inicial

El plan inicial mencionaba Catalog, Ordering, Payments, Identity y Admin. Se recomienda añadir al menos:

- **ShoppingCart / Checkout** (esencial para la experiencia de compra).
- **Inventory** (reserva y control de stock).
- **Shipping** (tarifas y fulfillment).
- **Promotions / Pricing**.
- **Notifications** (email/SMS, plantillas y colas).
- **Media / FileStorage** (imágenes y assets).
- **BackgroundJobs** y handlers de webhooks.
- **Search/Indexing** y **Reporting** (según necesidades de búsqueda y analítica).

Recomendación: priorizar ShoppingCart, Inventory y Shipping inmediatamente después de los módulos ya planificados; añadir Promotions y Notifications antes de integrar pagos en producción.

---

# Eventos de dominio recomendados

- ProductViewed
- ProductUpdated
- CartUpdated
- CartAbandoned
- InventoryReserved
- InventoryReleased
- OrderCreated
- OrderPlaced
- OrderCanceled
- PaymentAuthorized
- PaymentCaptured
- PaymentFailed
- PaymentRefunded
- ShipmentCreated
- ShipmentShipped
- ShipmentDelivered

---

# Próximos pasos sugeridos

1. Decidir si el MVP necesita pagos reales (sandbox) o solo pedidos sin cobro. Esto afecta prioridad de Payments y Shipping.
2. Implementar en orden: Identity → Catalog → ShoppingCart/Checkout → Ordering → Inventory → Payments (modo sandbox) → Notifications → Admin.
3. Definir contratos de eventos y Sagas para coordinar procesos distribuidos (p. ej. reserva+pago+envío).