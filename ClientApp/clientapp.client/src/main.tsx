import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import React from 'react'
import ProductsPageRouter from './products/ProductsPageRouter'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
        <ProductsPageRouter />
  </StrictMode>,
)
