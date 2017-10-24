## 0.4.0

### Features

* .NET Standard 1.1 support
* Geometry Types
  * CircularString
  * CompoundCurve
  * CurvePolygon
  * MultiCurve
  * MultiSurface
  * PolyhedralSurface
  * TIN
  * Triangle

### Breaking Changes

* **Polygon**: change type of ```ExteriorRing``` from ```List<Point>``` to ```LinearRing```
* **Polygon**: change type of ```InteriorRings``` from ```List<List<Point>>``` to ```List<LinearRing>```
* **MultiPoint**: rename ```Points``` -> ```Geometries```
* **MultiLineString**: rename ```LineStrings``` -> ```Geometries```
* **MultiPolygon**: rename ```Polygons``` -> ```Geometries```

## 0.3.0

### Features

* **Geometry**: add ```GetCenter``` method
* **Geometry**: add ```GetBoundingBox``` method
* **Geometry**: add ```SerializeString``` method
* **Geometry**: add ```SerializeByteArray``` method