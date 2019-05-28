## 0.5.1

### Bug Fixes

* Improve WKT and EWKT reading performance
* Fix serialization difference between .NET Core and .NET Framework with double.NaN values

## 0.5.0

### Features

* **Geometry**: add ```CurveToLine``` method

## 0.4.2

### Bug Fixes

* WKT de-serialization fails when M and Z values are present [#2](https://github.com/cschwarz/wkx-sharp/issues/2)

## 0.4.1

### Bug Fixes

* Unsupported WKT Formats [#1](https://github.com/cschwarz/wkx-sharp/issues/1)

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